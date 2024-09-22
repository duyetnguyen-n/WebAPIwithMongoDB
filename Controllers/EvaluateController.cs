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
    public class EvaluateController : Controller
    {
        private readonly IEvaluateRepository _Evaluate;

        public EvaluateController(IEvaluateRepository Evaluate)
        {
            _Evaluate = Evaluate;
        }
        [HttpGet]
        public async Task<IEnumerable<Evaluate>> GetEvaluates()
        {
            return await _Evaluate.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Evaluate))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetEvaluate(string id)
        {
            if (!await _Evaluate.Exists(id))
                return NotFound();
            var Evaluate = await _Evaluate.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Evaluate);
        }
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostEvaluate(Evaluate Evaluate)
        {

            await _Evaluate.CreateAsync(new Evaluate
            {
                UserId = Evaluate.UserId,
                CriteriaId = Evaluate.CriteriaId,
                Quantity = Evaluate.Quantity,
                TotalPointSubstraction = Evaluate.TotalPointSubstraction,
                TotalPointAddition = Evaluate.TotalPointAddition
            });

            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteEvaluate(string id)
        {
            if (!await _Evaluate.Exists(id))
                return NotFound();

            await _Evaluate.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}