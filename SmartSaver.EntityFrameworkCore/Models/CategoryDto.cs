using System.ComponentModel.DataAnnotations;

namespace SmartSaver.EntityFrameworkCore.Models
{
    public class CategoryDto
    {
        [Key]
        public int Id { get; set; }
        public string Title { get; set; }
        public bool TypeOfIncome { get; set; }
    }


}
