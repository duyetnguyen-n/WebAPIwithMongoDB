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
    public class CriteriaController : Controller
    {
        private readonly ICriteriaRepository _Criteria;

        public CriteriaController(ICriteriaRepository Criteria)
        {
            _Criteria = Criteria;
        }
        [HttpGet]
        public async Task<ActionResult<ApiResponse<IEnumerable<Criteria>>>> GetCriterias()
        {
            var criteria = await _Criteria.GetAsync();
            return Ok(new ApiResponse<IEnumerable<Criteria>>(200, "Thành công", criteria));
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200, Type = typeof(Criteria))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<ActionResult<ApiResponse<Criteria>>> GetCriteria(string id)
        {
            if (!await _Criteria.Exists(id))
                return NotFound(new ApiResponse<Criteria>(404, "Không tìm thấy tiêu chí", null));
            var Criteria = await _Criteria.GetAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Criteria>(200, "Thành công", Criteria));
        }

        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Criteria>>> PostCriteria(Criteria Criteria)
        {
            if (Criteria == null || !ModelState.IsValid)
            {
                return BadRequest(new ApiResponse<Criteria>(400, "Thất bại", null));
            }
            await _Criteria.CreateAsync(new Criteria
            {
                Name = Criteria.Name,
                Points = Criteria.Points,
                CriteriaGroupId = Criteria.CriteriaGroupId,
                Notes = Criteria.Notes,
                PersonCheck = Criteria.PersonCheck
            });

            return Ok(new ApiResponse<Criteria>(200, "Thành công", Criteria));
        }

        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Criteria>>> PutCriteria(Criteria Criteria)
        {
            if (!await _Criteria.Exists(Criteria.Id))
                return NotFound(new ApiResponse<Criteria>(404, "Không tìm thấy tiêu chí", null));
            var Criteriaold = await _Criteria.GetAsync(Criteria.Id);

            Criteriaold.Name = Criteria.Name;
            Criteriaold.Points = Criteria.Points;
            Criteriaold.CriteriaGroupId = Criteria.CriteriaGroupId;
            Criteriaold.Notes = Criteria.Notes;
            Criteriaold.PersonCheck = Criteria.PersonCheck;

            await _Criteria.UpdateAsync(Criteria.Id, Criteria);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(new ApiResponse<Criteria>(200, "Thành công", Criteria));
        }
        [HttpDelete("{id}")]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCriteria(string id)
        {
            if (!await _Criteria.Exists(id))
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy tiêu chí", null));

            await _Criteria.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            return Ok(new ApiResponse<string>(200, "Xóa thành công", null));
        }
    }
}