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
            CreateMap<CategoryRequestModel, Category>();
            CreateMap<Category, CategoryResponseModel>();

            CreateMap<TransactionRequestModel, Transaction>();
            CreateMap<Transaction, TransactionResponseModel>();
        }
    }
}
