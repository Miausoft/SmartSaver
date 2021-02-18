using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SmartSaver.WebApi.Controllers
{
    [ApiController]
    public class CategoryController : Controller
    {
        private readonly IRepository<Category> _categories;
        private readonly IMapper _mapper;

        public CategoryController(IRepository<Category> categories, IMapper mapper)
        {
            _categories = categories;
            _mapper = mapper;
        }

        [HttpGet("categories")]
        public async Task<IEnumerable<CategoryResponseModel>> Index()
        {
            return _mapper.Map<IEnumerable<CategoryResponseModel>>(
                await _categories.GetAll().ToListAsync());
        }

        [HttpGet("category/{categoryId}")]
        public async Task<CategoryResponseModel> Get(int categoryId)
        {
            return _mapper.Map<CategoryResponseModel>(
                await _categories.GetByIdAsync(categoryId));
        }

        [HttpPost("categories")]
        [CheckForInvalidModel]
        public async Task<IActionResult> Create(CategoryRequestModel category)
        {
            await _categories.InsertAsync(_mapper.Map<Category>(category));
            await _categories.SaveAsync();
            return Created($"category/{category}", category);
        }
    }
}
