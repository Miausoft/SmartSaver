using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;
using SmartSaver.EntityFrameworkCore.Models;
using SmartSaver.WebApi.RequestModels;
using SmartSaver.WebApi.ResponseModels;

namespace SmartSaver.WebApi
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<CategoryRequestModel, CategoryDto>();
            CreateMap<CategoryDto, CategoryResponseModel>();

            CreateMap<TransactionRequestModel, TransactionDto>();
            CreateMap<TransactionDto, TransactionResponseModel>();
        }
    }
}
