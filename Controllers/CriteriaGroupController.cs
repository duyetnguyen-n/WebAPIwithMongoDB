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
    public class CriteriaGroupController : Controller
    {
        private readonly ICriteriaGroupRepository _CriteriaGroup;

        public CriteriaGroupController(ICriteriaGroupRepository CriteriaGroup)
        {
            _CriteriaGroup = CriteriaGroup;
        }
        [HttpGet]
        public async Task<IEnumerable<CriteriaGroup>> GetCriteriaGroups()
        {
            return await _CriteriaGroup.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CriteriaGroup))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCriteriaGroup(string id)
        {
            if (!await _CriteriaGroup.Exists(id))
                return NotFound();
            var CriteriaGroup = await _CriteriaGroup.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(CriteriaGroup);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostCriteriaGroup(CriteriaGroup CriteriaGroup)
        {

            await _CriteriaGroup.CreateAsync(new CriteriaGroup
            {
                Name = CriteriaGroup.Name,
                Count = 0,
                Role = CriteriaGroup.Role
            });

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutCriteriaGroup(CriteriaGroup CriteriaGroup)
        {
            if (!await _CriteriaGroup.Exists(CriteriaGroup.Id))
                return NotFound();
            var CriteriaGroupold = await _CriteriaGroup.GetAsync(CriteriaGroup.Id);

            CriteriaGroupold.Name = CriteriaGroup.Name;
            CriteriaGroupold.Role = CriteriaGroup.Role;

            await _CriteriaGroup.UpdateAsync(CriteriaGroup.Id, CriteriaGroup);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteCriteriaGroup(string id)
        {
            if (!await _CriteriaGroup.Exists(id))
                return NotFound();

            await _CriteriaGroup.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}