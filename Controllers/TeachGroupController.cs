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
    public class TeachGroupController : Controller
    {
        private readonly ITeachGroupRepository _TeachGroup;

        public TeachGroupController(ITeachGroupRepository TeachGroup)
        {
            _TeachGroup = TeachGroup;
        }
        [HttpGet]
        public async Task<IEnumerable<TeachGroup>> GetTeachGroups()
        {
            return await _TeachGroup.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(TeachGroup))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTeachGroup(string id)
        {
            if (!await _TeachGroup.Exists(id))
                return NotFound();
            var TeachGroup = await _TeachGroup.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(TeachGroup);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostTeachGroup(TeachGroup TeachGroup)
        {

            await _TeachGroup.CreateAsync(new TeachGroup
            {
                Name = TeachGroup.Name,
                Count = 0
            });

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutTeachGroup(TeachGroup TeachGroup)
        {
            if (!await _TeachGroup.Exists(TeachGroup.Id))
                return NotFound();
            var TeachGroupold = await _TeachGroup.GetAsync(TeachGroup.Id);

            TeachGroupold.Name = TeachGroup.Name;

            await _TeachGroup.UpdateAsync(TeachGroup.Id, TeachGroup);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteTeachGroup(string id)
        {
            if (!await _TeachGroup.Exists(id))
                return NotFound();

            await _TeachGroup.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}