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
    public class PermissionRequestsController : Controller
    {
        private readonly IPermissionRequestsRepository _PermissionRequests;
        private readonly IEvaluateRepository _evaluateRepository;

        public PermissionRequestsController(IPermissionRequestsRepository PermissionRequests, IEvaluateRepository evaluateRepository)
        {
            _PermissionRequests = PermissionRequests;
            _evaluateRepository = evaluateRepository;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<PermissionRequests>>>> GetPermissionRequestss()
        {
            var PermissionRequests = await _PermissionRequests.GetAsync();
            return Ok(new ApiResponse<IEnumerable<PermissionRequests>>(200, "Thành công", PermissionRequests));
        }
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PermissionRequests))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]


        public async Task<ActionResult<ApiResponse<PermissionRequests>>> GetPermissionRequests(string id)
        {
            if (!await _PermissionRequests.Exists(id))
                return NotFound(new ApiResponse<PermissionRequests>(404, "Không tìm thấy yêu cầu", null));
            var PermissionRequests = await _PermissionRequests.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<PermissionRequests>(200, "Thành công", PermissionRequests));
        }
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<PermissionRequests>>> PostPermissionRequests(PermissionRequests PermissionRequests)
        {
            if (PermissionRequests == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<PermissionRequests>(400, "Thất bại", null));
            }
            await _PermissionRequests.CreateAsync(new PermissionRequests
            {
                UserId = PermissionRequests.UserId,
                RequetedPermissionId  = PermissionRequests.RequetedPermissionId,
                TimeStamp = DateTime.Now,
                Status = PermissionRequests.Status,
                ReviewerId = PermissionRequests.ReviewerId,
                Description = PermissionRequests.Description,
                Comments = PermissionRequests.Comments
            });

            return Ok(new ApiResponse<PermissionRequests>(200, "Thành công", PermissionRequests));
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<PermissionRequests>>> PutPermissionRequests(PermissionRequests PermissionRequests)
        {
            if (!await _PermissionRequests.Exists(PermissionRequests.Id))
                return NotFound(new ApiResponse<PermissionRequests>(404, "Không tìm thấy tiêu chí", null));
            var PermissionRequestsold = await _PermissionRequests.GetAsync(PermissionRequests.Id);
            var valuate = await _evaluateRepository.GetAsync(PermissionRequestsold.RequetedPermissionId);
            if(valuate ==null)
                return NotFound(new ApiResponse<PermissionRequests>(404, "Không tìm thấy đánh giá", null));

            if(PermissionRequestsold.ReviewerId != PermissionRequests.ReviewerId)
                PermissionRequestsold.ReviewerId = PermissionRequests.ReviewerId;
            if(PermissionRequestsold.Status != PermissionRequests.Status)
                PermissionRequestsold.Status = PermissionRequests.Status;
            PermissionRequestsold.TimeStamp = DateTime.Now;
            valuate.ConfirmDay = DateTime.Now;

            await _PermissionRequests.UpdateAsync(PermissionRequests.Id, PermissionRequests);
            await _evaluateRepository.UpdateAsync(valuate.Id, valuate);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<PermissionRequests>(200, "Thành công", PermissionRequests));
        }


        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeletePermissionRequests(string id)
        {
            if (!await _PermissionRequests.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy yêu cầu", null));

            await _PermissionRequests.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
    }
}