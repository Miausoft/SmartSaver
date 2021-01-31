using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.Domain.Repositories;
using SmartSaver.Domain.CustomAttributes;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;
using AutoMapper;

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
        public IEnumerable<CategoryResponseModel> Index()
        {
            return _mapper.Map<IEnumerable<CategoryResponseModel>>(
                _categories.GetAll());
        }

        [HttpGet("category/{categoryId}")]
        public CategoryResponseModel Get(int categoryId)
        {
            return _mapper.Map<CategoryResponseModel>(
                _categories.GetById(categoryId));
        }

        [HttpPost("categories")]
        [CheckForInvalidModel]
        public ActionResult Create(CategoryRequestModel category)
        {
            _categories.Insert(_mapper.Map<Category>(category));
            _categories.Save();
            return Created($"category/{category}", category);
        }
    }
}
