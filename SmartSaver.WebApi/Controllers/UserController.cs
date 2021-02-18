using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.WebApi.Controllers
{
    public class UserController : Controller
    {
        private readonly IRepository<User> _users;

        public UserController(IRepository<User> users)
        {
            _users = users;
        }

        [HttpGet("users")]
        public async Task<IEnumerable<User>> Index()
        {
            return await _users.GetAll().ToListAsync();
        }

        [HttpGet("user/{userId}")]
        public async Task<User> Get(int userId)
        {
            return await _users.GetByIdAsync(userId);
        }

        [HttpPost("users")]
        public async Task<IActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            await _users.InsertAsync(user);
            await _users.SaveAsync();
            return Ok();
        }
    }
}
