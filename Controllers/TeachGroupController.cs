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
    public class TeachGroupController : Controller
    {
        private readonly ITeachGroupRepository _TeachGroup;
        private readonly IUserRepository _user;

        public TeachGroupController(ITeachGroupRepository TeachGroup, IUserRepository user)
        {
            _TeachGroup = TeachGroup;
            _user = user;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<TeachGroup>>>> GetTeachGroups()
        {
            var TeachGroups = await _TeachGroup.GetAsync();
            return Ok(new ApiResponse<IEnumerable<TeachGroup>>(200, "Thành công", TeachGroups));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TeachGroup))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<TeachGroup>>> GetTeachGroup(string id)
        {
            if (!await _TeachGroup.Exists(id))
                return NotFound(new ApiResponse<TeachGroup>(404, "Không tìm thấy tổ", null));
            var TeachGroup = await _TeachGroup.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<TeachGroup>(200, "Thành công", TeachGroup));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<TeachGroup>>> PostTeachGroup(TeachGroup TeachGroup)
        {
            if (TeachGroup == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<TeachGroup>(400, "Thất bại", null));
            }
            await _TeachGroup.CreateAsync(new TeachGroup
            {
                Name = TeachGroup.Name,
                Count = 0
            });

            return Ok(new ApiResponse<TeachGroup>(200, "Thành công", TeachGroup));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<TeachGroup>>> PutTeachGroup(TeachGroup TeachGroup)
        {
            if (!await _TeachGroup.Exists(TeachGroup.Id))
                return NotFound(new ApiResponse<TeachGroup>(404, "Không tìm thấy tổ", null));
            var TeachGroupold = await _TeachGroup.GetAsync(TeachGroup.Id);

            TeachGroupold.Name = TeachGroup.Name;

            await _TeachGroup.UpdateAsync(TeachGroup.Id, TeachGroup);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<TeachGroup>(200, "Thành công", TeachGroup));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteTeachGroup(string id)
        {
            if (!await _TeachGroup.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy tổ", null));

            var users = await _user.GetUsersByTeachGroupId(id);
            if (users.Any())
            {
                return BadRequest(new ApiResponse<string>(400, "Xóa không thành công vì còn người bên trong", null));
            }

            await _TeachGroup.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(400, "Xóa không thành công vì có lỗi gì rồi", null));

            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }

    }
}