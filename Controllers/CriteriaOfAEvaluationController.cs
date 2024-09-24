using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Services;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriteriaOfAEvaluationController : Controller
    {
        private readonly ICriteriaOfAEvaluationRepository _CriteriaOfAEvaluation;

        public CriteriaOfAEvaluationController(ICriteriaOfAEvaluationRepository CriteriaOfAEvaluation)
        {
            _CriteriaOfAEvaluation = CriteriaOfAEvaluation;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CriteriaOfAEvaluation>>>> GetCriteriaOfAEvaluations()
        {
            var criteriaOfAEvaluation = await _CriteriaOfAEvaluation.GetAsync();
            return Ok(new ApiResponse<IEnumerable<CriteriaOfAEvaluation>>(200, "Thành công", criteriaOfAEvaluation));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CriteriaOfAEvaluation))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<CriteriaOfAEvaluation>>> GetCriteriaOfAEvaluation(string id)
        {
            if (!await _CriteriaOfAEvaluation.Exists(id))
                return NotFound(new ApiResponse<Criteria>(404, "Không tìm thấy tiêu chí", null));
            var CriteriaOfAEvaluation = await _CriteriaOfAEvaluation.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(200, "Lỗi không lấy được", null));

            return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", CriteriaOfAEvaluation));
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<CriteriaOfAEvaluation>>> PostCriteriaOfAEvaluation(CriteriaOfAEvaluation CriteriaOfAEvaluation)
        {
            if (CriteriaOfAEvaluation == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(400, "Thất bại", null));
            }
            await _CriteriaOfAEvaluation.CreateAsync(new CriteriaOfAEvaluation
            {
                EvaluateId = CriteriaOfAEvaluation.EvaluateId,
                CriteriaId = CriteriaOfAEvaluation.CriteriaId,
                Quantity = CriteriaOfAEvaluation.Quantity,
                Total = CriteriaOfAEvaluation.Total
            });

            return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", CriteriaOfAEvaluation));
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<CriteriaOfAEvaluation>>> DeleteCriteriaOfAEvaluation(string id)
        {
            if (!await _CriteriaOfAEvaluation.Exists(id))
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy tiêu chí", null));

            await _CriteriaOfAEvaluation.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(400, "Thất bại", null));
                
            return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", null));
        }
    }
}