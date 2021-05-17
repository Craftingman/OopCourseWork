using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace PrisonHeadDirectory.Models
{
    public class RegisterUserViewModel
    {
        [Required(ErrorMessage = "Не указан Email")]
        [EmailAddress (ErrorMessage = "Некорректный адрес")]
        [Remote(action: "CheckEmail", controller: "Users", ErrorMessage ="Пользователь с таким E-Mail уже зарегистрирован")]
        public string Email { get; set; }
 
        [Required(ErrorMessage = "Не указан пароль")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
 
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }
        
        [Required(ErrorMessage = "Не указана роль пользователя")]
        public string RoleName { get; set; }
        
        [Required(ErrorMessage = "Не указано имя")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина имени должна быть от 1 до 50 символов")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Не указано отчество")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Длина отчества должна быть от 3 до 50 символов")]
        public string MiddleName { get; set; }

        [Required(ErrorMessage = "Не указана фамилия")]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина фамилии должна быть от 1 до 50 символов")]
        public string Surname { get; set; }
    }
}