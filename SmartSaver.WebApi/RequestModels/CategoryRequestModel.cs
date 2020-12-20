using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.WebApi.RequestModels
{
    public class CategoryRequestModel
    {
        [Required(ErrorMessage = "Title is required")]
        [Display(Name = "Title:")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Type is required")]
        [Display(Name = "Income type:")]
        public bool TypeOfIncome { get; set; }
    }
}
