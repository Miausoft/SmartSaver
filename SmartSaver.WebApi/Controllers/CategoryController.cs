using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.ActionFilters;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;

namespace SmartSaver.WebApi.Controllers
{
    [ApiController]
    public class CategoryController : Controller
    {
        public CategoryController(ICategoryRepository categories, IMapper mapper)
        {
            _categories = categories;
            _mapper = mapper;
        }

        private ICategoryRepository _categories;
        private IMapper _mapper;

        [HttpGet("categories")]
        public IEnumerable<CategoryResponseModel> Index()
        {
            return _mapper.Map<IEnumerable<CategoryResponseModel>>(
                _categories.GetMultiple()
            );
        }

        [HttpGet("category/{categoryId}")]
        public CategoryResponseModel Get(int categoryId)
        {
            return _mapper.Map<CategoryResponseModel>(
                _categories.GetSingle(c => c.Id == categoryId)
            );
        }

        [HttpPost("categories")]
        [CheckForInvalidModel]
        public async Task<ActionResult> Create(CategoryRequestModel category)
        {
            int createdId = await _categories.CreateAsync(_mapper.Map<Category>(category));
            return Created($"category/{createdId}", category);
        }
    }
}
