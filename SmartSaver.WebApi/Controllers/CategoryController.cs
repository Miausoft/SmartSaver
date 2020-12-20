using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.WebApi.CustomExceptions;

namespace SmartSaver.WebApi.Controllers
{
    public class CategoryController : Controller
    {
        public CategoryController(ICategoryRepo categories)
        {
            _categories = categories;
        }

        private ICategoryRepo _categories;

        [HttpGet("categories")]
        public IEnumerable<Category> Index()
        {
            return _categories.GetMultiple();
        }

        [HttpGet("category/{categoryId}")]
        public Category Get(int categoryId)
        {
            return _categories.GetSingle(c => c.Id == categoryId);
        }

        [HttpPost("categories")]
        public async Task<ActionResult> Create([Bind("Title, TypeOfIncome")] Category category)
        {
            if (!ModelState.IsValid)
            {
                throw new InvalidModelException();
            }

            int createdId = await _categories.CreateAsync(category);
            return Created($"category/{createdId}", category);
        }
    }
}
