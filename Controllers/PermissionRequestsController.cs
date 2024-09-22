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

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionRequestsController : Controller
    {
        private readonly IPermissionRequestsRepository _PermissionRequests;

        public PermissionRequestsController(IPermissionRequestsRepository PermissionRequests)
        {
            _PermissionRequests = PermissionRequests;
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IEnumerable<PermissionRequests>> GetPermissionRequestss()
        {
            return await _PermissionRequests.GetAsync();
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PermissionRequests))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPermissionRequests(string id)
        {
            if (!await _PermissionRequests.Exists(id))
                return NotFound();
            var PermissionRequests = await _PermissionRequests.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(PermissionRequests);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostPermissionRequests(PermissionRequests PermissionRequests)
        {

            await _PermissionRequests.CreateAsync(new PermissionRequests
            {
                UserId = PermissionRequests.UserId,
                RequetedPermissionId  = PermissionRequests.RequetedPermissionId,
                TimeStamp = PermissionRequests.TimeStamp,
                Status = PermissionRequests.Status,
                ReviewerId = PermissionRequests.ReviewerId,
                Description = PermissionRequests.Description,
                Comments = PermissionRequests.Comments
            });

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeletePermissionRequests(string id)
        {
            if (!await _PermissionRequests.Exists(id))
                return NotFound();

            await _PermissionRequests.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}