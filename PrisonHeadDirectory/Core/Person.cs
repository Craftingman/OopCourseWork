using System.ComponentModel.DataAnnotations;

namespace Core
{
    public abstract class Person : BaseEntity
    {
        [Required(ErrorMessage = "Не указано имя")]
        public string Name { get; set; }
        
        [Required(ErrorMessage = "Не указана фамилия")]
        public string Surname { get; set; }
        
        [Required(ErrorMessage = "Не указано отчество")]
        public string MiddleName { get; set; }
    }
}