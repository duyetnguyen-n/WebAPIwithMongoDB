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
    public class PermissionController : Controller
    {
        private readonly IPermissionRepository _Permission;

        public PermissionController(IPermissionRepository Permission)
        {
            _Permission = Permission;
        }
        [HttpGet]
        public async Task<IEnumerable<Permission>> GetPermissions()
        {
            return await _Permission.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Permission))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPermission(string id)
        {
            if (!await _Permission.Exists(id))
                return NotFound();
            var Permission = await _Permission.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Permission);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostPermission(Permission Permission)
        {

            await _Permission.CreateAsync(new Permission
            {
                Name = Permission.Name

            });

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutPermission(Permission Permission)
        {
            if (!await _Permission.Exists(Permission.Id))
                return NotFound();
            var Permissionold = await _Permission.GetAsync(Permission.Id);

            Permissionold.Name = Permission.Name;

            await _Permission.UpdateAsync(Permission.Id, Permission);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeletePermission(string id)
        {
            if (!await _Permission.Exists(id))
                return NotFound();

            await _Permission.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}