using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Article : BaseEntity
    {
        [Required(ErrorMessage = "Не указано название")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Длина названия должна быть от 5 до 100 символов")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Не указан номер")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Длина номера должна быть от 1 до 10 символов")]
        public string Number { get; set; }
    }
}