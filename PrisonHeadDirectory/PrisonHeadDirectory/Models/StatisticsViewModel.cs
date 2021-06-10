using System.Collections.Generic;
using Core;

namespace PrisonHeadDirectory.Models
{
    public class StatisticsViewModel
    {
        public int TotalPrisoners { get; set; }
        public Dictionary<Article, int> ArticlePrisoners { get; set; }
        public Dictionary<Caste, int> CastePrisoners { get; set; }
        public Dictionary<string, int> TermPrisoners { get; set; }  
    }
}