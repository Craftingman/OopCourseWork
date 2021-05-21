using System.Collections.Generic;
using System.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrisonHeadDirectory.Models;

namespace PrisonHeadDirectory.Controllers
{
    public class StatisticsController : Controller
    {
        private readonly IPrisonerDalService _prisonerDalService;
        private readonly ICasteDalService _casteDalService;
        private readonly IArticleDalService _articleDalService;
        private readonly IRelativeDalService _relativeDalService;

        public StatisticsController(
            IPrisonerDalService prisonerDalService,
            ICasteDalService casteDalService,
            IArticleDalService articleDalService,
            IRelativeDalService relativeDalService)
        {
            _prisonerDalService = prisonerDalService;
            _casteDalService = casteDalService;
            _articleDalService = articleDalService;
            _relativeDalService = relativeDalService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            var prisoners = _prisonerDalService.GetPrisoners().ToList();
            var castes = _casteDalService.GetAll();
            var articles = _articleDalService.GetAll();

            StatisticsViewModel statistics = new StatisticsViewModel()
            {
                TotalPrisoners = prisoners.Count(),
                
                ArticlePrisoners = articles.GroupJoin(prisoners,
                    a => a.Id,
                    p => p.ArticleId,
                    (a, prs) => new
                    {
                        Article = a,
                        PrisonerCount = prs.Count()
                    }).ToDictionary(el => el.Article,
                    el => el.PrisonerCount),
                
                CastePrisoners = castes.GroupJoin(prisoners,
                    c => c.Id,
                    p => p.CasteId,
                    (c, prs) => new
                    {
                        Caste = c,
                        PrisonerCount = prs.Count()
                    }).ToDictionary(el => el.Caste,
                    el => el.PrisonerCount),
                
                TermPrisoners = new Dictionary<string, int>(new List<KeyValuePair<string, int>>(
                    new KeyValuePair<string, int>[]
                    {
                        new KeyValuePair<string, int>("< 1 года", prisoners
                            .Count(p => (p.ReleaseDate - p.ArrestDate).TotalDays < 365)),
                        new KeyValuePair<string, int>(">= 1 и < 10 лет", prisoners
                            .Count(p => (p.ReleaseDate - p.ArrestDate).TotalDays is < 3650 and >= 365)),
                        new KeyValuePair<string, int>(">= 10 лет", prisoners
                            .Count(p => (p.ReleaseDate - p.ArrestDate).TotalDays > 3650)),
                    }))
            };
            
            return View(statistics);
        }
    }
}