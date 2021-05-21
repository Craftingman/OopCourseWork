using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Core
{
    public class Prisoner : Person
    {
        [Required(ErrorMessage = "Не указана дата освобождения")]
        public DateTime ReleaseDate { get; set; }
        
        [Required(ErrorMessage = "Не указана дата ареста")]
        public DateTime ArrestDate { get; set; }

        public string Notes { get; set; } = string.Empty;
        
        [Required(ErrorMessage = "Не указана камера")]
        [StringLength(10, MinimumLength = 1, ErrorMessage = "Длина названия должна быть от 1 до 10 символов")]
        public string Cell { get; set; }

        public string ImgPath { get; set; } = "";

        //Relations

        public int CasteId { get; set; }

        public int ArticleId { get; set; }

        public ICollection<Relative> Relatives { get; set; } = new List<Relative>();
    }
}