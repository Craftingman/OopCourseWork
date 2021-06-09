using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PrisonHeadDirectory.Models;

namespace PrisonHeadDirectory.Controllers
{
    public class PrisonerController : Controller
    {
        private readonly IPrisonerDalService _prisonerDalService;
        private readonly ICasteDalService _casteDalService;
        private readonly IArticleDalService _articleDalService;
        private readonly IRelativeDalService _relativeDalService;

        public PrisonerController(
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
        public IActionResult Get(string searchStr = "", int articleId = 0, int casteId = 0,
            string arrestDateFromStr = null, string arrestDateToStr = null)
        {
            searchStr ??= "";
            DateTime arrestDateFrom = arrestDateFromStr is null ? DateTime.MinValue
                : DateTime.Parse(arrestDateFromStr, CultureInfo.CurrentCulture);
            DateTime arrestDateTo = arrestDateToStr is null ? DateTime.MaxValue 
                : DateTime.Parse(arrestDateToStr, CultureInfo.CurrentCulture);


            IEnumerable<Prisoner> prisoners = _prisonerDalService.GetPrisoners(searchStr);
            List<PrisonerShort> prisonerShorts = new List<PrisonerShort>();
            
            var castes = _casteDalService.GetAll().ToList();
            castes.Add(new Caste()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });
            
            SelectList castesList = 
                new SelectList(castes, "Id", "Name", 0);

            var articles = _articleDalService.GetAll().ToList();
            articles.Add(new Article()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });
            
            SelectList articlesList = 
                new SelectList(articles, "Id", "Name", 0);

            foreach (var prisoner in prisoners)
            {
                prisonerShorts.Add(new PrisonerShort()
                {
                        Name = prisoner.Name,
                        Surname = prisoner.Surname,
                        MiddleName = prisoner.MiddleName,
                        ArrestDate = prisoner.ArrestDate,
                        ReleaseDate = prisoner.ReleaseDate,
                        ArticleIds = prisoner.Articles.Select(a => a.Id).ToList(),
                        CasteId = prisoner.CasteId,
                        Id = prisoner.Id
                });
            }

            if (casteId != 0)
            {
                prisonerShorts = prisonerShorts.Where(ps => ps.CasteId == casteId).ToList();
            }
            
            if (articleId != 0)
            {
                prisonerShorts = prisonerShorts.Where(ps => ps.ArticleIds.Contains(articleId)).ToList();
            }

            prisonerShorts = prisonerShorts.Where(ps => DateTime.Compare(ps.ArrestDate, (DateTime)arrestDateFrom) > 0
            && DateTime.Compare(ps.ArrestDate, (DateTime)arrestDateTo) < 0)
                .ToList();

            ViewBag.Articles = articlesList;
            ViewBag.Castes = castesList;

            return View(prisonerShorts);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(Prisoner prisoner)
        {
            if (ModelState.IsValid)
            {
                _prisonerDalService.Create(prisoner);
                return RedirectToAction("Edit", new {id = prisoner.Id});
            }

            return View(prisoner);
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            Prisoner prisoner = _prisonerDalService.Get(id);

            var castes = _casteDalService.GetAll().ToList();

            castes.Add(new Caste()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });

            if (!castes.Select(c => c.Id).Contains(prisoner.CasteId))
            {
                prisoner.CasteId = 0;
            }

            SelectList castesList = 
                new SelectList(castes, "Id", "Name", prisoner.CasteId);
            
            ViewBag.Castes = castesList;
            
            return View(prisoner);
        }
        
        [HttpPost]
        public IActionResult Edit(Prisoner prisoner)
        {
            if (ModelState.IsValid)
            {
                Prisoner currentPrisoner = _prisonerDalService.Get(prisoner.Id);

                if (currentPrisoner is null)
                {
                    return View(prisoner);
                }

                prisoner.Articles = currentPrisoner.Articles;
                _prisonerDalService.Update(prisoner);
                
                return RedirectToAction("Edit", new {id = prisoner.Id});
            }

            return View(prisoner);
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _prisonerDalService.Delete(id);
            return RedirectToAction("Get");
        }
        
        [HttpGet]
        public IActionResult DisplayPrisonerInfo(int id)
        {
            Prisoner prisoner = _prisonerDalService.Get(id);
            return PartialView("_PrisonerInfo", prisoner);
        }

        [HttpGet]
        public IActionResult DeleteRelative(int id, int prisonerId)
        {
            _relativeDalService.Delete(id);
            return RedirectToAction("Edit", new {id = prisonerId});
        }
        
        [HttpPost]
        public IActionResult CreateRelative(Relative relative)
        {
            _relativeDalService.Add(relative);
            return null;
        }
        
        public IActionResult DeleteArticle(int id, int prisonerId)
        {
            Prisoner prisoner = _prisonerDalService.Get(prisonerId);
            prisoner.Articles.Remove(prisoner.Articles.FirstOrDefault(a => a.Id == id));
            
            _prisonerDalService.Update(prisoner);
            
            return RedirectToAction("Edit", new {id = prisonerId});
        }
        
        [HttpGet]
        public IActionResult AddArticle(int prisonerId)
        {
            Prisoner prisoner = _prisonerDalService.Get(prisonerId);

            if (prisoner is null)
            {
                return RedirectToAction("Edit", new {id = prisonerId});
            }
            
            return RedirectToAction("SelectArticle", "Article", new
            {
                nextAction = Url.Action("AddArticle", new {prisonerId = prisonerId}),
                exceptIds = prisoner.Articles.Select(a => a.Id).ToList()
            });
        }
        
        [HttpPost]
        public IActionResult AddArticle(int prisonerId, int articleId)
        {
            Prisoner prisoner = _prisonerDalService.Get(prisonerId);
            Article article = _articleDalService.Get(articleId);
            
            if (prisoner is null || article is null)
            {
                return Json(new { redirectToUrl = Url.Action("Edit", new {id = prisonerId}) });
            }
            
            prisoner.Articles.Add(article);
            _prisonerDalService.Update(prisoner);
            
            return Json(new { redirectToUrl = Url.Action("Edit", new {id = prisonerId}) });
        }
    }
}