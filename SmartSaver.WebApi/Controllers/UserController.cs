using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;

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
        public IEnumerable<User> Index()
        {
            return _users.GetAll();
        }

        [HttpGet("user/{userId}")]
        public User Get(int userId)
        {
            return _users.GetById(userId);
        }

        [HttpPost("users")]
        public ActionResult Create(User user)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            _users.Insert(user);
            _users.Save();
            return Ok();
        }
    }
}
