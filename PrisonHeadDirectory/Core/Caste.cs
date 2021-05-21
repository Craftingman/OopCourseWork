using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Caste : BaseEntity
    {
        [Required(ErrorMessage = "Не указано название")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина названия должна быть от 1 до 50 символов")]
        public string Name { get; set; }
    }
}