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
    public class LogController : Controller
    {
        private readonly ILogRepository _Log;

        public LogController(ILogRepository Log)
        {
            _Log = Log;
        }
        [HttpGet]
        public async Task<IEnumerable<Log>> GetLogs()
        {
            return await _Log.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Log))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetLog(string id)
        {
            if (!await _Log.Exists(id))
                return NotFound();
            var Log = await _Log.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(Log);
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostLog(Log Log)
        {

            await _Log.CreateAsync(new Log
            {
                UserId = Log.UserId,
                Action = Log.Action,
                TimeStamp = Log.TimeStamp,
                Status = Log.Status,
                Description = Log.Description
            });

            return NoContent();
        }

        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteLog(string id)
        {
            if (!await _Log.Exists(id))
                return NotFound();

            await _Log.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok();
        }
    }
}