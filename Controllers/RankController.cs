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
    public class RankController : Controller
    {
        private readonly IRankRepository _Rank;

        public RankController(IRankRepository Rank)
        {
            _Rank = Rank;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Rank>>>> GetRanks()
        {
            var ranks = await _Rank.GetAsync();
            return Ok(new ApiResponse<IEnumerable<Rank>>(200, "Thành công", ranks));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Rank))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Rank>>> GetRank(string id)
        {
            if (!await _Rank.Exists(id))
                return NotFound(new ApiResponse<Rank>(404, "Không tìm thấy nhóm tiêu chí", null));
            var Rank = await _Rank.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Rank>(200, "Thành công", Rank));
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Rank>>> PostRank(Rank Rank)
        {
            if (Rank == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Rank>(400, "Thất bại", null));
            }
            await _Rank.CreateAsync(new Rank
            {
                Name = Rank.Name,
                PointRangeStart = Rank.PointRangeStart,
                PointRangeEnd = Rank.PointRangeEnd,
                TimeStamp = DateTime.Now

            });

            return Ok(new ApiResponse<Rank>(200, "Thành công", Rank));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<Rank>>> PutRank(Rank Rank)
        {
            if (!await _Rank.Exists(Rank.Id))
                return NotFound(new ApiResponse<CriteriaGroup>(404, "Không tìm thấy nhóm tiêu chí", null));
            var Rankold = await _Rank.GetAsync(Rank.Id);

            Rankold.Name = Rank.Name;
            Rankold.PointRangeStart = Rank.PointRangeStart;
            Rankold.PointRangeEnd = Rank.PointRangeEnd;

            await _Rank.UpdateAsync(Rank.Id, Rank);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Rank>(200, "Thành công", Rank));
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult<ApiResponse<string>>> DeleteRank(string id)
        {
            if (!await _Rank.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy nhóm tiêu chí", null));

            await _Rank.DeleteAsync(id);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }

    }
}