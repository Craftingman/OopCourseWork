using System;
using Core;

namespace PrisonHeadDirectory.Models
{
    public class PrisonerShort : Person
    {
        public DateTime ReleaseDate { get; set; }

        public DateTime ArrestDate { get; set; }
        
        public int CasteId { get; set; }

        public int ArticleId { get; set; }
    }
}