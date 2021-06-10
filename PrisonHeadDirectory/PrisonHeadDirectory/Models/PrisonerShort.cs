using System;
using System.Collections.Generic;
using Core;

namespace PrisonHeadDirectory.Models
{
    public class PrisonerShort : Person
    {
        public DateTime ReleaseDate { get; set; }

        public DateTime ArrestDate { get; set; }
        
        public int CasteId { get; set; }

        public ICollection<int> ArticleIds { get; set; }
    }
}