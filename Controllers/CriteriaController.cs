using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Services;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriteriaController : ControllerBase
    {
        private readonly CriteriaService _criteriaService;

        public CriteriaController(CriteriaService criteriaService)
        {
            _criteriaService = criteriaService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Criteria>>>> GetCriterias()
        {
            var criteriaList = await _criteriaService.GetCriteriasAsync();
            return new ApiResponse<IEnumerable<Criteria>>(200, "Thành công", criteriaList);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Criteria>>> GetCriteria(string id)
        {
            var criteria = await _criteriaService.GetCriteriaByIdAsync(id);
            if (criteria == null)
            {
                return new ApiResponse<Criteria>(404, "Không tìm thấy tiêu chí", null);
            }

            return new ApiResponse<Criteria>(200, "Thành công", criteria);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Criteria>>> PostCriteria(Criteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<Criteria>(400, "Dữ liệu không hợp lệ", null);
            }

            var newCriteria = await _criteriaService.AddCriteriaAsync(criteria);
            if (newCriteria == null)
            {
                return new ApiResponse<Criteria>(400, "Thêm thất bại", null);
            }

            return new ApiResponse<Criteria>(200, "Thêm thành công", newCriteria);
        }

        [HttpPut]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Criteria>>> PutCriteria(Criteria criteria)
        {
            if (!ModelState.IsValid)
            {
                return new ApiResponse<Criteria>(400, "Dữ liệu không hợp lệ", null);
            }

            var updatedCriteria = await _criteriaService.UpdateCriteriaAsync(criteria);
            if (updatedCriteria == null)
            {
                return new ApiResponse<Criteria>(404, "Không tìm thấy tiêu chí", null);
            }

            return new ApiResponse<Criteria>(200, "Sửa thành công", updatedCriteria);
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCriteria(string id)
        {
            var deleted = await _criteriaService.DeleteCriteriaAsync(id);
            if (!deleted)
            {
                return new ApiResponse<string>(404, "Không tìm thấy tiêu chí", null);
            }

            return new ApiResponse<string>(200, "Xóa thành công", null);
        }
    }
}
