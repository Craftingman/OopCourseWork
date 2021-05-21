using System.Collections;
using System.Collections.Generic;
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

        public string GetArticleName(int id)
        {
            Article article = _articleDalService.Get(id);
            if (article is null)
            {
                return "-- не выбрано --";
            }

            return $"{article.Number} - {article.Name}";
        }
    }
}