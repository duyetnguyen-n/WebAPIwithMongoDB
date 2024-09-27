using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Services;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EvaluateController : Controller
    {
        private readonly IEvaluateRepository _Evaluate;
        private readonly ICriteriaOfAEvaluationRepository _CriteriaOfAEvaluate;
        private readonly IUserRepository _user;
        private readonly IRankRepository _rankRepository;

        public EvaluateController(IEvaluateRepository Evaluate, ICriteriaOfAEvaluationRepository CriteriaOfAEvaluate, IUserRepository user, IRankRepository rankRepository)
        {
            _Evaluate = Evaluate;
            _rankRepository = rankRepository;
            _user = user;
            _CriteriaOfAEvaluate = CriteriaOfAEvaluate;
        }
        [HttpGet("all")]
        [ProducesResponseType(200, Type = typeof(ApiResponse<IEnumerable<Evaluate>>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<IEnumerable<Evaluate>>>> GetAllEvaluates()
        {
            var Evaluate = await _Evaluate.GetAsync();
            

            return Ok(new ApiResponse<IEnumerable<Evaluate>>(200, "Thành công", Evaluate));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Evaluate))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Evaluate>>> GetEvaluate(string id)
        {
            if (!await _Evaluate.Exists(id))
                return NotFound(new ApiResponse<Evaluate>(404, "Không tìm thấy đánh giá này", null));
            var Evaluate = await _Evaluate.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Evaluate>(200, "Thành công", Evaluate));
        }
        [HttpGet("User/{userId}")]
        [ProducesResponseType(200, Type = typeof(Evaluate))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<IEnumerable<Evaluate>>>> GetEvaluateByUserId(string userId)
        {
            if (!ObjectId.TryParse(userId, out ObjectId objectId))
            {
                return BadRequest(new ApiResponse<IEnumerable<Evaluate>>(200, "Không đúng định dạng", null));
            }
            var Evaluate = await _Evaluate.GetEvaluationsByUserId(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<IEnumerable<Evaluate>>(200, "Thành công", Evaluate));
        }
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Evaluate>>> PostEvaluate(Evaluate Evaluate)
        {
            if (Evaluate == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Evaluate>(400, "Thất bại", null));
            }
            var isAdmin = User.IsInRole("Admin");

            DateTime? confirmDay = isAdmin ? DateTime.Now : (DateTime?)null; 
            var createdEvaluate = new Evaluate
            {
                Name = Evaluate.Name,
                UserId = Evaluate.UserId,
                RankId = Evaluate.RankId,
                TotalPointSubstraction = 0,
                TotalPointAddition = 0,
                From = Evaluate.From,
                To = Evaluate.To,
                TotalPoint = 0,
                TimeStamp = DateTime.Now,
                UploadDay = DateTime.Now,
                ConfirmDay = confirmDay
            };

            await _Evaluate.CreateAsync(createdEvaluate);
            return Ok(new ApiResponse<Evaluate>(200, "Thành công", createdEvaluate));

        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Evaluate>>> PutEvaluate(Evaluate Evaluate)
        {
            var existingEvaluate = await _Evaluate.GetAsync(Evaluate.Id);
            if (existingEvaluate == null)
                return NotFound(new ApiResponse<Evaluate>(404, "Không tìm thấy đánh giá", null));

            if (existingEvaluate.Name != Evaluate.Name)
                existingEvaluate.Name = Evaluate.Name;

            if (existingEvaluate.UserId != Evaluate.UserId)
                existingEvaluate.UserId = Evaluate.UserId;

            if (existingEvaluate.TotalPointSubstraction != Evaluate.TotalPointSubstraction)
                existingEvaluate.TotalPointSubstraction = Evaluate.TotalPointSubstraction;

            if (existingEvaluate.TotalPointAddition != Evaluate.TotalPointAddition)
                existingEvaluate.TotalPointAddition = Evaluate.TotalPointAddition;

            if (existingEvaluate.From != Evaluate.From)
                existingEvaluate.From = Evaluate.From;

            if (existingEvaluate.To != Evaluate.To)
                existingEvaluate.To = Evaluate.To;

            if (existingEvaluate.ConfirmDay != Evaluate.ConfirmDay)
                existingEvaluate.ConfirmDay = Evaluate.ConfirmDay;

            existingEvaluate.TimeStamp = DateTime.Now;

            await _Evaluate.UpdateAsync(Evaluate.Id, existingEvaluate);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Evaluate>(400, "Không thành công", existingEvaluate));

            return Ok(new ApiResponse<Evaluate>(200, "Thành công", existingEvaluate));
        }


        [HttpPut("PutEvaluateStt/{Id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Evaluate>>> PutEvaluateSTT(string Id, [FromBody] int stt)
        {
            var evaluate = await _Evaluate.GetAsync(Id);
            if (evaluate == null)
                return NotFound(new ApiResponse<Evaluate>(404, "Không tìm thấy tiêu chí", null));

            await _Evaluate.UpdateEvaluateSTT(Id, stt);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Evaluate>(400, "Không thành công", null));

            return Ok(new ApiResponse<Evaluate>(200, "Thành công", evaluate));
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteEvaluate(string id)
        {
            var evaluate = await _Evaluate.GetAsync(id);
            if (evaluate == null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy đánh giá", null));
            var criteriaOfAEvaluation = await _CriteriaOfAEvaluate.GetCriteriesByEvaluateId(id);
            foreach(var item in criteriaOfAEvaluation){
                await _CriteriaOfAEvaluate.DeleteAsync(item.Id);
            }
            var user = await _user.GetAsync(evaluate.UserId);
            if(user ==null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy người dùng", null));
            user.Point -= evaluate.TotalPoint;
            await _Evaluate.DeleteAsync(id);
            await _user.UpdateAsync(user.Id, user);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
    }
}