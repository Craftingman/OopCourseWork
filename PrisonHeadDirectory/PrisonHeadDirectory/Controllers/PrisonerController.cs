using System;
using System.Collections;
using System.Collections.Generic;
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
        public IActionResult Get()
        {
            IEnumerable<Prisoner> prisoners = _prisonerDalService.GetPrisoners();
            List<PrisonerShort> prisonerShorts = new List<PrisonerShort>();
            /*
            var castes = _casteDalService.GetAll().ToList();
            var articles = _articleDalService.GetAll().ToList();

            castes.Add(new Caste()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });
            
            articles.Add(new Article()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });

            SelectList castesList = 
                new SelectList(castes, "Id", "Name",0);

            SelectList articlesList =
                new SelectList(articles, "Id", "Name", 0);

            ViewBag.Articles = articlesList;
            ViewBag.Castes = castesList;
            */
            foreach (var prisoner in prisoners)
            {
                prisonerShorts.Add(new PrisonerShort()
                {
                        Name = prisoner.Name,
                        Surname = prisoner.Surname,
                        MiddleName = prisoner.MiddleName,
                        ArrestDate = prisoner.ArrestDate,
                        ReleaseDate = prisoner.ReleaseDate,
                        ArticleId = prisoner.ArticleId,
                        CasteId = prisoner.CasteId,
                        Id = prisoner.Id
                });
            }
            
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
            var articles = _articleDalService.GetAll().ToList();

            castes.Add(new Caste()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });
            
            articles.Add(new Article()
            {
                Name = "-- Не выбрано --",
                Id = 0
            });
            
            if (!castes.Select(c => c.Id).Contains(prisoner.CasteId))
            {
                prisoner.CasteId = 0;
            }
            
            if (!articles.Select(a => a.Id).Contains(prisoner.ArticleId))
            {
                prisoner.ArticleId = 0;
            }
            
            SelectList castesList = 
                new SelectList(castes, "Id", "Name", prisoner.CasteId);

            SelectList articlesList =
                new SelectList(articles, "Id", "Name", prisoner.ArticleId);
            
            ViewBag.Articles = articlesList;
            ViewBag.Castes = castesList;
            
            return View(prisoner);
        }
        
        [HttpPost]
        public IActionResult Edit(Prisoner prisoner)
        {
            if (ModelState.IsValid)
            {
                _prisonerDalService.Update(prisoner);
                
                return RedirectToAction("Edit", new {id = prisoner.Id});
            }

            return View(prisoner);
        }
        
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
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
    }
}