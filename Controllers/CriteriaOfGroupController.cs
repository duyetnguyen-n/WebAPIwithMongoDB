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
    public class CriteriaOfGroupController : Controller
    {
        private readonly ICriteriaOfGroupRepository _CriteriaOfGroup;

        public CriteriaOfGroupController(ICriteriaOfGroupRepository CriteriaOfGroup)
        {
            _CriteriaOfGroup = CriteriaOfGroup;
        }
        [HttpGet]
        public async Task<IEnumerable<CriteriaOfGroup>> GetCriteriaOfGroups()
        {
            return await _CriteriaOfGroup.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(CriteriaOfGroup))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCriteriaOfGroup(string id)
        {
            if (!await _CriteriaOfGroup.Exists(id))
                return NotFound();
            var CriteriaOfGroup = await _CriteriaOfGroup.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(CriteriaOfGroup);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostCriteriaOfGroup(CriteriaOfGroup CriteriaOfGroup)
        {

            await _CriteriaOfGroup.CreateAsync(new CriteriaOfGroup
            {
                CriteriaGroupId = CriteriaOfGroup.CriteriaGroupId,
                CriteriaId = CriteriaOfGroup.CriteriaId,
                
            });

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteCriteriaOfGroup(string id)
        {
            if (!await _CriteriaOfGroup.Exists(id))
                return NotFound();

            await _CriteriaOfGroup.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}