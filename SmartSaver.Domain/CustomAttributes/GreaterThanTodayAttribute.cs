using System;
using System.ComponentModel.DataAnnotations;

namespace SmartSaver.Domain.CustomAttributes
{
    public class GreaterThanTodayAttribute : ValidationAttribute
    {
        public GreaterThanTodayAttribute() { }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            return (DateTime)value > DateTime.Now.Date ?
                ValidationResult.Success :
                new ValidationResult(ErrorMessage ?? $"The date must be greater than { DateTime.Now.ToShortDateString() }");
        }
    }
}
