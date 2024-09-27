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
using WebAPIwithMongoDB.Services;

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
        public async Task<ActionResult<ApiResponse<IEnumerable<Log>>>> GetLogs()
        {
            var logs = await _Log.GetAsync();
            return Ok(new ApiResponse<IEnumerable<Log>>(200, "Thành công", logs));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Log))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Log>>> GetLog(string id)
        {
            if (await _Log.GetAsync(id) != null)
                return NotFound(new ApiResponse<Log>(404, "Không tìm thấy log này", null));
            var Log = await _Log.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Log>(200, "Thành công", Log));
        }

        [HttpGet("user/{useriId}")]
        [ProducesResponseType(200, Type = typeof(IEnumerable<Log>))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<IEnumerable<Log>>>> GetLogsByUserId(string userId)
        {
            
            var Log = await _Log.GetLogByUserId(userId);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<IEnumerable<Log>>(200, "Thành công", Log));
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Log>>> PostLog(Log Log)
        {
            if (Log == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Log>(400, "Thất bại", null));
            }
            await _Log.CreateAsync(new Log
            {
                UserId = Log.UserId,
                Action = Log.Action,
                TimeStamp = Log.TimeStamp,
                Status = Log.Status,
                Description = Log.Description
            });
            return Ok(new ApiResponse<Log>(200, "Thành công", Log));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteLog(string id)
        {
            if (await _Log.GetAsync(id) == null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy nhóm tiêu chí", null));

            await _Log.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [HttpDelete("delete-multiple")]
        public async Task<IActionResult> DeleteMultipleLogs([FromBody] List<string> ids)
        {
            try
            {
                await _Log.DeleteMultipleAsync(ids);
                return Ok(new ApiResponse<string>(200, "Xóa thành công các mục được chọn", null));
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiResponse<string>(200, "Lỗi rồi còn đâu", null));
            }
        }
    }
}