using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Relative : Person
    {
        [Required(ErrorMessage = "Не указан адрес")]
        [StringLength(100, MinimumLength = 5, ErrorMessage = "Длина адреса должна быть от 5 до 100 символов")]
        public string Address { get; set; }
        
        [Required(ErrorMessage = "Не указан номер телефона")]
        [StringLength(20, MinimumLength = 5, ErrorMessage = "Длина номера должна быть от 5 до 20 символов")]
        [DataType(DataType.PhoneNumber)]
        public string PhoneNumber { get; set; }
        
        [Required(ErrorMessage = "Не указана роль родственника")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина роли должна быть от 3 до 50 символов")]
        public string RelativeRole { get; set; }

        public int PrisonerId { get; set; }
    }
}