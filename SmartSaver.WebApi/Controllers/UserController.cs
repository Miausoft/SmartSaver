using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;

namespace SmartSaver.WebApi.Controllers
{
    public class UserController : Controller
    {
        public UserController(IUserRepository users)
        {
            _users = users;
        }

        private readonly IUserRepository _users;

        [HttpGet("users")]
        public IEnumerable<User> Index()
        {
            return _users.Get();
        }

        [HttpGet("user/{userId}")]
        public User Get(int userId)
        {
            return _users.GetSingle(u => u.Id == userId);
        }

        [HttpPost("users")]
        public async Task<ActionResult> Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(CreateUserResponse.StateNotValid);
            }

            var result = await _users.CreateAsync(user);
            return Ok(result);
        }
    }
}
