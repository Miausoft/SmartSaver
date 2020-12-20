using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmartSaver.WebApi.ResponseModels
{
    public class CategoryResponseModel
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public bool TypeOfIncome { get; set; }
    }
}
