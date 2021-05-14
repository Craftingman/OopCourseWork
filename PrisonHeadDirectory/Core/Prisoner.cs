using System;
using System.Collections;
using System.Collections.Generic;

namespace Core
{
    public class Prisoner : Person
    {
        public TimeSpan Term { get; set; }

        public DateTime ArrestDate { get; set; }

        public string Notes { get; set; }

        public string Cell { get; set; }

        public string ImgPath { get; set; }

        //Relations

        public Caste Caste { get; set; }

        public Article Article { get; set; }

        public ICollection<Relative> Relatives { get; set; }
    }
}