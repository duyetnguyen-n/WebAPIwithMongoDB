using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
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
        private readonly IEvaluateRepository _evaluationRepository;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;

        public CriteriaOfAEvaluationController(ICriteriaOfAEvaluationRepository CriteriaOfAEvaluation, ICriteriaGroupRepository criteriaGroupRepository, ICriteriaRepository criteriaRepository, IEvaluateRepository evaluationRepository)
        {
            _CriteriaOfAEvaluation = CriteriaOfAEvaluation;
            _evaluationRepository = evaluationRepository;
            _criteriaRepository = criteriaRepository;
            _criteriaGroupRepository = criteriaGroupRepository;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CriteriaOfAEvaluation>>>> GetCriteriaOfAEvaluations()
        {
            var criteriaOfAEvaluation = await _CriteriaOfAEvaluation.GetAsync();
            return Ok(new ApiResponse<IEnumerable<CriteriaOfAEvaluation>>(200, "Thành công", criteriaOfAEvaluation));
        }

        [HttpGet("{evaluateId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<CriteriaOfAEvaluation>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CriteriaOfAEvaluation>>>> GetCriteriaOfAEvaluation(string evaluateId)
        {
            var criteriaOfAEvaluation = await _CriteriaOfAEvaluation.GetCriteriesByEvaluateId(evaluateId);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<IEnumerable<CriteriaOfAEvaluation>>(400, "Lỗi không lấy được", null));

            return Ok(new ApiResponse<IEnumerable<CriteriaOfAEvaluation>>(200, "Thành công", criteriaOfAEvaluation));
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
            try{
                Evaluate evaluate = new Evaluate(); 
                var EvaluateOld = await _evaluationRepository.GetAsync(CriteriaOfAEvaluation.EvaluateId);
                var CriteriaOld = await _criteriaRepository.GetAsync(CriteriaOfAEvaluation.CriteriaId);
                var CriteriaCroupOld = await _criteriaGroupRepository.GetAsync(CriteriaOld.CriteriaGroupId);

                if (CriteriaCroupOld.Role.ToLower() == "trừ điểm")
                    evaluate.TotalPointSubstraction = EvaluateOld.TotalPointSubstraction + CriteriaOfAEvaluation.Total;
                else
                {
                    evaluate.TotalPointAddition = EvaluateOld.TotalPointAddition + CriteriaOfAEvaluation.Total;
                }


                evaluate.TotalPoint =  EvaluateOld.TotalPoint + EvaluateOld.TotalPointAddition - EvaluateOld.TotalPointSubstraction;

                await _evaluationRepository.UpdateAsync(CriteriaOfAEvaluation.EvaluateId, evaluate);

                await _CriteriaOfAEvaluation.CreateAsync(new CriteriaOfAEvaluation
                {
                    EvaluateId = CriteriaOfAEvaluation.EvaluateId,
                    CriteriaId = CriteriaOfAEvaluation.CriteriaId,
                    Quantity = CriteriaOfAEvaluation.Quantity,
                    Total = CriteriaOfAEvaluation.Total,
                    TimeStamp = DateTime.Now
                });

                return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", CriteriaOfAEvaluation));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CriteriaOfAEvaluation>(500, $"Lỗi server: {ex.Message}", null));
            }

            
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