using System.Collections.Generic;
using System.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace PrisonHeadDirectory.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleDalService _articleDalService;

        public ArticleController(IArticleDalService articleDalService)
        {
            _articleDalService = articleDalService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Article> articles = _articleDalService.GetAll();
            return View(articles);
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _articleDalService.Delete(id);
            return RedirectToAction("Get");
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Article article)
        {
            if (ModelState.IsValid)
            {
                _articleDalService.Add(article);

                return RedirectToAction("Get");
            }

            return View(article);
        }

        public string GetArticleNames(int[] ids)
        {
            List<Article> articles = new List<Article>();

            foreach (var id in ids)
            {
                Article article = _articleDalService.Get(id);

                if (article is null)
                {
                    continue;
                }
                
                articles.Add(article);
            }

            if (!articles.Any())
            {
                return "-- не выбрано --";
            }

            return string.Join(", ", articles.Select(a => a.Name));
        }
        
        [HttpGet]
        public IActionResult SelectArticle(string nextAction, IEnumerable<int> exceptIds)
        {
            ViewBag.NextAction = nextAction;
            IEnumerable<Article> articles = _articleDalService.GetAll()
               .Where(a => !exceptIds.Contains(a.Id)).ToList();
            
            return PartialView("_SelectArticle", articles);
        }
    }
}