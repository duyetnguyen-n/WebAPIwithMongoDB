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
    public class PositionController : Controller
    {
        private readonly IPositionRepository _position;

        public PositionController(IPositionRepository position)
        {
            _position = position;
        }
        [HttpGet]
        public async Task<IEnumerable<Position>> GetPositions()
        {
            return await _position.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Position))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPosition(string id)
        {
            if (!await _position.Exists(id))
                return NotFound();
            var Position = await _position.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Position);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostPosition(Position Position)
        {

            await _position.CreateAsync(new Position
            {
                Name = Position.Name

            });

            return NoContent();
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutPosition(Position Position)
        {
            if (!await _position.Exists(Position.Id))
                return NotFound();
            var Positionold = await _position.GetAsync(Position.Id);

            Positionold.Name = Position.Name;

            await _position.UpdateAsync(Position.Id, Position);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeletePosition(string id)
        {
            if (!await _position.Exists(id))
                return NotFound();

            await _position.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}