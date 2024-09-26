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
                UploadDay = DateTime.Now
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
            if (!await _Evaluate.Exists(Evaluate.Id))
                return NotFound(new ApiResponse<Evaluate>(404, "Không tìm thấy tiêu chí", null));

            var Evaluateold = await _Evaluate.GetAsync(Evaluate.Id);
            if (Evaluateold == null)
                return NotFound(new ApiResponse<Evaluate>(404, "Không tìm thấy đánh giá", null));

            Evaluateold.Name = Evaluate.Name;
            Evaluateold.UserId = Evaluate.UserId;
            Evaluateold.TotalPointSubstraction = Evaluate.TotalPointSubstraction;
            Evaluateold.TotalPointAddition = Evaluate.TotalPointAddition;
            Evaluateold.From = Evaluate.From;
            Evaluateold.To = Evaluate.To;
            Evaluateold.TimeStamp = DateTime.Now;
            Evaluateold.ConfirmDay = Evaluate.ConfirmDay;

            await _Evaluate.UpdateAsync(Evaluate.Id, Evaluateold);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<Evaluate>(400, "Không thành công", Evaluateold));

            return Ok(new ApiResponse<Evaluate>(200, "Thành công", Evaluateold));
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
            user.Point -= Convert.ToInt32(evaluate.TotalPoint);
            await _Evaluate.DeleteAsync(id);
            await _user.UpdateAsync(user.Id, user);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
    }
}