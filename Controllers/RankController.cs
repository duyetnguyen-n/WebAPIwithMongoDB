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
    public class RankController : Controller
    {
        private readonly IRankRepository _Rank;

        public RankController(IRankRepository Rank)
        {
            _Rank = Rank;
        }
        [HttpGet]
        public async Task<IEnumerable<Rank>> GetRanks()
        {
            return await _Rank.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Rank))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetRank(string id)
        {
            if (!await _Rank.Exists(id))
                return NotFound();
            var Rank = await _Rank.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Rank);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostRank(Rank Rank)
        {

            await _Rank.CreateAsync(new Rank
            {
                Name = Rank.Name,
                PointRangeStart = Rank.PointRangeStart,
                PointRangeEnd = Rank.PointRangeEnd

            });

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutRank(Rank Rank)
        {
            if (!await _Rank.Exists(Rank.Id))
                return NotFound();
            var Rankold = await _Rank.GetAsync(Rank.Id);

            Rankold.Name = Rank.Name;
            Rankold.PointRangeStart = Rank.PointRangeStart;
            Rankold.PointRangeEnd = Rank.PointRangeEnd;

            await _Rank.UpdateAsync(Rank.Id, Rank);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteRank(string id)
        {
            if (!await _Rank.Exists(id))
                return NotFound();

            await _Rank.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}