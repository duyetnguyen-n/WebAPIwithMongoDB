using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using WebAPIwithMongoDB.Entities;
using WebAPIwithMongoDB.Repositories.Interface;

namespace WebAPIwithMongoDB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : Controller
    {
        private readonly IUserRepository _users;

        public UserController(IUserRepository users)
        {
            _users = users;
        }
        [HttpGet]
        public async Task<IEnumerable<User>> GetUsers()
        {
            return await _users.GetAsync();
        }

        [HttpGet("{id}")]
        [ProducesResponseType(200,Type = typeof(User))]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetUser(string id){
            if (!await _users.Exists(id))
                return NotFound();
            var user = _users.GetAsync(id);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);
            
            return Ok(user);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ProducesResponseType(400)]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<ActionResult> PostUser(User user){

            await _users.CreateAsync(new User
            {
                NumberPhone = user.NumberPhone,
                Password = user.Password,
                Position = user.Position,
                Name = user.Name,
                Age = user.Age,
                Mail = user.Mail,
                Gender = user.Gender,
                TeachGroupId = user.TeachGroupId,
                Point = user.Point,
                Avatar = user.Avatar,
                
            });

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> PutUser(User user){
            if (!await _users.Exists(user.Id))
                return NotFound();
            var userold = await _users.GetAsync(user.Id);

            userold.NumberPhone = user.NumberPhone;
            userold.Password = user.Password;
            userold.Position = user.Position;
            userold.Name = user.Name;
            userold.Age = user.Age;
            userold.Mail = user.Mail;
            userold.Gender = user.Gender;
            userold.TeachGroupId = user.TeachGroupId;
            userold.Point = user.Point;
            userold.Avatar = user.Avatar;

            await _users.UpdateAsync(user.Id, user);

            if(!ModelState.IsValid)
                return BadRequest(ModelState);

            return NoContent();
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(200)]
        public async Task<ActionResult> DeleteUser(string id)
        {
            if (!await _users.Exists(id))
                return NotFound();

            await _users.DeleteAsync(id);

            if (!ModelState.IsValid)
                return BadRequest(ModelState);   
            return Ok();
        }

        
    }
}