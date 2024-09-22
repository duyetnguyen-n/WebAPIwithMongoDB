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
    public class CriteriaController : Controller
    {
        private readonly ICriteriaRepository _Criteria;

        public CriteriaController(ICriteriaRepository Criteria)
        {
            _Criteria = Criteria;
        }
        [HttpGet]
        public async Task<IEnumerable<Criteria>> GetCriterias()
        {
            return await _Criteria.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Criteria))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetCriteria(string id)
        {
            if (!await _Criteria.Exists(id))
                return NotFound();
            var Criteria = await _Criteria.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Criteria);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PostCriteria(Criteria Criteria)
        {

            await _Criteria.CreateAsync(new Criteria
            {
                Name = Criteria.Name,
                Points = Criteria.Points,
                CriteriaGroupId = Criteria.CriteriaGroupId,
                Notes = Criteria.Notes,
                PersonCheck = Criteria.PersonCheck
            });

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> PutCriteria(Criteria Criteria)
        {
            if (!await _Criteria.Exists(Criteria.Id))
                return NotFound();
            var Criteriaold = await _Criteria.GetAsync(Criteria.Id);

            Criteriaold.Name = Criteria.Name;
            Criteriaold.Points = Criteria.Points;
            Criteriaold.CriteriaGroupId = Criteria.CriteriaGroupId;
            Criteriaold.Notes = Criteria.Notes;
            Criteriaold.PersonCheck = Criteria.PersonCheck;

            await _Criteria.UpdateAsync(Criteria.Id, Criteria);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> DeleteCriteria(string id)
        {
            if (!await _Criteria.Exists(id))
                return NotFound();

            await _Criteria.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}