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
        private readonly IRankRepository _rankRepository;
        private readonly ICriteriaRepository _criteriaRepository;
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;
        private readonly IUserRepository _userRepoistory;

        public CriteriaOfAEvaluationController(
        ICriteriaOfAEvaluationRepository CriteriaOfAEvaluation, 
        IUserRepository userRepoistory, 
        IRankRepository rankRepository, 
        ICriteriaGroupRepository criteriaGroupRepository, 
        ICriteriaRepository criteriaRepository, 
        IEvaluateRepository evaluationRepository)
        {
            _CriteriaOfAEvaluation = CriteriaOfAEvaluation;
            _evaluationRepository = evaluationRepository;
            _criteriaRepository = criteriaRepository;
            _criteriaGroupRepository = criteriaGroupRepository;
            _rankRepository = rankRepository;
            _userRepoistory = userRepoistory;
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
        public async Task<ActionResult<ApiResponse<CriteriaOfAEvaluation>>> PostCriteriaOfAEvaluation(CriteriaOfAEvaluation criteriaOfAEvaluation)
        {
            if (criteriaOfAEvaluation == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(400, "Thất bại", null));
            }

            try
            {
                if (string.IsNullOrEmpty(criteriaOfAEvaluation.EvaluateId) || string.IsNullOrEmpty(criteriaOfAEvaluation.CriteriaId))
                {
                    return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(400, "EvaluateId hoặc CriteriaId không hợp lệ", null));
                }

                var evaluateOld = await _evaluationRepository.GetAsync(criteriaOfAEvaluation.EvaluateId);
                if (evaluateOld == null)
                {
                    return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Evaluate", null));
                }

                var criteriaOld = await _criteriaRepository.GetAsync(criteriaOfAEvaluation.CriteriaId);
                if (criteriaOld == null)
                {
                    return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Criteria", null));
                }
                var user = await _userRepoistory.GetAsync(evaluateOld.UserId);

                var criteriaGroupOld = await _criteriaGroupRepository.GetAsync(criteriaOld.CriteriaGroupId);
                if (criteriaGroupOld == null)
                {
                    return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Criteria Group", null));
                }

                var totalPoint = Convert.ToInt32(criteriaOfAEvaluation.Quantity * criteriaOld.Points);

                if (criteriaGroupOld.Role.ToLower() == "trừ điểm")
                {
                    evaluateOld.TotalPointSubstraction += totalPoint;
                    if (evaluateOld.ConfirmDay != null)
                    {
                        user.Point -= Convert.ToInt32(totalPoint) ;       
                    }
                }
                else
                {
                    evaluateOld.TotalPointAddition += totalPoint;
                    if (evaluateOld.ConfirmDay != null)
                    {
                        user.Point += Convert.ToInt32(totalPoint);
                    }
                }

                evaluateOld.TotalPoint = evaluateOld.TotalPointAddition - evaluateOld.TotalPointSubstraction;

                var ranks = await _rankRepository.GetAsync();
                var rankId = ranks.FirstOrDefault(rank =>
                    Convert.ToInt32(evaluateOld.TotalPoint) >= Convert.ToInt32(rank.PointRangeStart) &&
                    Convert.ToInt32(evaluateOld.TotalPoint) <= Convert.ToInt32(rank.PointRangeEnd))?.Id;

                evaluateOld.RankId = rankId;
                await _userRepoistory.UpdateAsync(evaluateOld.UserId, user);



                await _evaluationRepository.UpdateAsync(criteriaOfAEvaluation.EvaluateId, evaluateOld);

                var newCriteria = new CriteriaOfAEvaluation
                {
                    EvaluateId = criteriaOfAEvaluation.EvaluateId,
                    CriteriaId = criteriaOfAEvaluation.CriteriaId,
                    Quantity = criteriaOfAEvaluation.Quantity,
                    Total = totalPoint,
                    TimeStamp = DateTime.Now
                };

                await _CriteriaOfAEvaluation.CreateAsync(newCriteria);

                return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", newCriteria));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<CriteriaOfAEvaluation>(500, $"Lỗi server: {ex.Message}", null));
            }
        }



        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<CriteriaOfAEvaluation>>> DeleteCriteriaOfAEvaluation(string id)
        {
            if (!await _CriteriaOfAEvaluation.Exists(id))
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy tiêu chí", null));

            var criteriaOfAEvaluation = await _CriteriaOfAEvaluation.GetAsync(id);
            if (criteriaOfAEvaluation == null)
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy tiêu chí", null));

            // Lấy Evaluate hiện tại
            var evaluate = await _evaluationRepository.GetAsync(criteriaOfAEvaluation.EvaluateId);
            if (evaluate == null)
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Evaluate", null));

            // Lấy thông tin của Criteria
            var criteria = await _criteriaRepository.GetAsync(criteriaOfAEvaluation.CriteriaId);
            if (criteria == null)
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Criteria", null));

            // Lấy thông tin của CriteriaGroup
            var criteriaGroup = await _criteriaGroupRepository.GetAsync(criteria.CriteriaGroupId);
            if (criteriaGroup == null)
                return NotFound(new ApiResponse<CriteriaOfAEvaluation>(404, "Không tìm thấy Criteria Group", null));

            // Tính tổng điểm của CriteriaOfAEvaluation
            var totalPoint = Convert.ToInt32(criteriaOfAEvaluation.Quantity * criteria.Points);

            // Cập nhật điểm cộng/trừ dựa trên vai trò của CriteriaGroup
            if (criteriaGroup.Role.ToLower() == "trừ điểm")
            {
                evaluate.TotalPointSubstraction -= totalPoint;
            }
            else
            {
                evaluate.TotalPointAddition -= totalPoint;
            }

            // Tính toán lại tổng điểm hiện tại của Evaluate
            evaluate.TotalPoint = evaluate.TotalPointAddition - evaluate.TotalPointSubstraction;

            // Cập nhật lại xếp hạng (Rank) dựa trên tổng điểm
            var ranks = await _rankRepository.GetAsync();
            var rankId = ranks.FirstOrDefault(rank =>
                Convert.ToInt32(evaluate.TotalPoint) >= Convert.ToInt32(rank.PointRangeStart) &&
                Convert.ToInt32(evaluate.TotalPoint) <= Convert.ToInt32(rank.PointRangeEnd))?.Id;
            evaluate.RankId = rankId;

            // Cập nhật Evaluate sau khi xóa CriteriaOfAEvaluation
            await _evaluationRepository.UpdateAsync(evaluate.Id, evaluate);

            // Cập nhật điểm của người dùng
            var user = await _userRepoistory.GetAsync(evaluate.UserId);
            user.Point -= totalPoint;
            await _userRepoistory.UpdateAsync(user.Id, user);

            // Thực hiện xóa CriteriaOfAEvaluation
            await _CriteriaOfAEvaluation.DeleteAsync(id);

            // Kiểm tra ModelState và trả về kết quả
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<CriteriaOfAEvaluation>(400, "Thất bại", null));

            return Ok(new ApiResponse<CriteriaOfAEvaluation>(200, "Thành công", null));
        }

        [HttpPost("update-points")]
        public async Task UpdatePointsWhenCriteriaGroupRoleChanges(string criteriaGroupId)
        {
            // Lấy tất cả các tiêu chí thuộc nhóm tiêu chí có ID là criteriaGroupId
            var criterias = await _criteriaRepository.GetCriteriesByCriteriaGroupId(criteriaGroupId);

            foreach (var criteria in criterias)
            {
                // Lấy tất cả các tiêu chí của đánh giá có CriteriaId tương ứng
                var criteriaOfEvaluations = await _CriteriaOfAEvaluation.GetEvaluationesByCriteriaId(criteria.Id);

                foreach (var criteriaOfEvaluation in criteriaOfEvaluations)
                {
                    // Lấy Evaluate liên quan
                    var evaluate = await _evaluationRepository.GetAsync(criteriaOfEvaluation.EvaluateId);
                    if (evaluate == null) continue;

                    // Tính lại tổng điểm của tiêu chí
                    var totalPoint = Convert.ToInt32(criteriaOfEvaluation.Quantity * criteria.Points);

                    // Lấy thông tin CriteriaGroup mới nhất
                    var criteriaGroup = await _criteriaGroupRepository.GetAsync(criteria.CriteriaGroupId);

                    // Nếu role mới là "trừ điểm", cập nhật tổng điểm trừ và tổng điểm chung
                    if (criteriaGroup.Role.ToLower() == "trừ điểm")
                    {
                        evaluate.TotalPointSubstraction += totalPoint;
                        evaluate.TotalPointAddition -= totalPoint;
                    }
                    else
                    {
                        evaluate.TotalPointAddition += totalPoint;
                        evaluate.TotalPointSubstraction -= totalPoint;
                    }

                    // Cập nhật lại tổng điểm
                    evaluate.TotalPoint = evaluate.TotalPointAddition - evaluate.TotalPointSubstraction;

                    // Cập nhật lại rank
                    var ranks = await _rankRepository.GetAsync();
                    var rankId = ranks.FirstOrDefault(rank =>
                        Convert.ToInt32(evaluate.TotalPoint) >= Convert.ToInt32(rank.PointRangeStart) &&
                        Convert.ToInt32(evaluate.TotalPoint) <= Convert.ToInt32(rank.PointRangeEnd))?.Id;
                    evaluate.RankId = rankId;

                    // Cập nhật Evaluate
                    await _evaluationRepository.UpdateAsync(evaluate.Id, evaluate);

                    // Cập nhật điểm cho user
                    var user = await _userRepoistory.GetAsync(evaluate.UserId);
                    user.Point = evaluate.TotalPoint;
                    await _userRepoistory.UpdateAsync(user.Id, user);
                }
            }
        }

    }
}