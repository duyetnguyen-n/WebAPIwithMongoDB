using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PermissionOfAPositionController : Controller
    {
        private readonly IPermissionOfAPositionRepository _PermissionOfAPosition;

        public PermissionOfAPositionController(IPermissionOfAPositionRepository PermissionOfAPosition)
        {
            _PermissionOfAPosition = PermissionOfAPosition;
        }
        [HttpGet]
        public async Task<IEnumerable<PermissionOfAPosition>> GetPermissionOfAPositions()
        {
            return await _PermissionOfAPosition.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(PermissionOfAPosition))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPermissionOfAPosition(string id)
        {
            if (!await _PermissionOfAPosition.Exists(id))
                return NotFound();
            var PermissionOfAPosition = await _PermissionOfAPosition.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(PermissionOfAPosition);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostPermissionOfAPosition(PermissionOfAPosition PermissionOfAPosition)
        {

            await _PermissionOfAPosition.CreateAsync(new PermissionOfAPosition
            {
                PositionId = PermissionOfAPosition.PositionId,
                PermissionId = PermissionOfAPosition.PermissionId

            });

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeletePermissionOfAPosition(string id)
        {
            if (!await _PermissionOfAPosition.Exists(id))
                return NotFound();

            await _PermissionOfAPosition.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}