using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;
using WebAPIwithMongoDB.Services;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CriteriaGroupController : ControllerBase
    {
        private readonly ICriteriaGroupRepository _criteriaGroupRepository;
        private readonly ICriteriaRepository _criteriaRepository;

        private readonly ICriteriaGroupRoleService _criteriaGroupRoleService;

        public CriteriaGroupController(ICriteriaGroupRepository criteriaGroupRepository, 
                                       ICriteriaRepository criteriaRepository, 
                                       ICriteriaGroupRoleService criteriaGroupRoleService)
        {
            _criteriaGroupRepository = criteriaGroupRepository;
            _criteriaRepository = criteriaRepository;
            _criteriaGroupRoleService = criteriaGroupRoleService;
        }

        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<CriteriaGroup>>>> GetCriteriaGroups()
        {
            var criteriaGroups = await _criteriaGroupRepository.GetAsync();
            return Ok(new ApiResponse<IEnumerable<CriteriaGroup>>(200, "Thành công", criteriaGroups));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CriteriaGroup>>> GetCriteriaGroup(string id)
        {
            if (!await _criteriaGroupRepository.Exists(id))
                return NotFound(new ApiResponse<CriteriaGroup>(404, "Không tìm thấy nhóm tiêu chí", null));

            var criteriaGroup = await _criteriaGroupRepository.GetAsync(id);
            return Ok(new ApiResponse<CriteriaGroup>(200, "Thành công", criteriaGroup));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CriteriaGroup>>> PostCriteriaGroup(CriteriaGroup criteriaGroup)
        {
            if (criteriaGroup == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<CriteriaGroup>(400, "Thất bại", null));
            }

            await _criteriaGroupRepository.CreateAsync(new CriteriaGroup
            {
                Name = criteriaGroup.Name,
                Count = 0,
                Role = criteriaGroup.Role,
                TimeStamp = DateTime.Now
            });

            return Ok(new ApiResponse<CriteriaGroup>(200, "Thành công", criteriaGroup));
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<ActionResult<ApiResponse<CriteriaGroup>>> PutCriteriaGroup(CriteriaGroup criteriaGroup)
        {
            if (!await _criteriaGroupRepository.Exists(criteriaGroup.Id))
                return NotFound(new ApiResponse<CriteriaGroup>(404, "Không tìm thấy nhóm tiêu chí", null));

            var criteriaGroupOld = await _criteriaGroupRepository.GetAsync(criteriaGroup.Id);
            criteriaGroupOld.Name = criteriaGroup.Name;
            criteriaGroupOld.Count = Convert.ToInt32(criteriaGroupOld.Count) + Convert.ToInt32(criteriaGroup.Count);
            criteriaGroupOld.Role = criteriaGroup.Role;
            criteriaGroupOld.TimeStamp = DateTime.Now;

            await _criteriaGroupRepository.UpdateAsync(criteriaGroup.Id, criteriaGroupOld);

            await _criteriaGroupRoleService.UpdatePointsWhenCriteriaGroupRoleChanges(criteriaGroup.Id);

            return Ok(new ApiResponse<CriteriaGroup>(200, "Cập nhật thành công", criteriaGroupOld));
        }

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCriteriaGroup(string id)
        {
            if (!await _criteriaGroupRepository.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy nhóm tiêu chí", null));

            var criteria = await _criteriaRepository.GetCriteriesByCriteriaGroupId(id);
            if (criteria.Any())
            {
                return BadRequest(new ApiResponse<string>(400, "Xóa không thành công vì còn tiêu chí bên trong", null));
            }

            await _criteriaGroupRepository.DeleteAsync(id);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }

        
    }
}
