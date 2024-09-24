using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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

        public EvaluateController(IEvaluateRepository Evaluate)
        {
            _Evaluate = Evaluate;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Evaluate>>>> GetEvaluates()
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
            await _Evaluate.CreateAsync(new Evaluate
            {
                UserId = Evaluate.UserId,
                CriteriaId = Evaluate.CriteriaId,
                Quantity = Evaluate.Quantity,
                TotalPointSubstraction = Evaluate.TotalPointSubstraction,
                TotalPointAddition = Evaluate.TotalPointAddition
            });

            return Ok(new ApiResponse<Evaluate>(200, "Thành công", Evaluate));
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteEvaluate(string id)
        {
            if (!await _Evaluate.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy đánh giá", null));

            await _Evaluate.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
    }
}