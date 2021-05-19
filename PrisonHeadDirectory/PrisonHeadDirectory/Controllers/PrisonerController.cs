using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;
using PrisonHeadDirectory.Models;

namespace PrisonHeadDirectory.Controllers
{
    public class PrisonerController : Controller
    {
        private readonly IPrisonerDalService _prisonerDalService;

        public PrisonerController(IPrisonerDalService prisonerDalService)
        {
            _prisonerDalService = prisonerDalService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Prisoner> prisoners = _prisonerDalService.GetPrisoners();
            List<PrisonerShort> prisonerShorts = new List<PrisonerShort>();
            
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
            return View(prisoner);
        }
        
        [HttpPost]
        public IActionResult Edit(Prisoner prisoner)
        {
            if (ModelState.IsValid)
            {
                _prisonerDalService.Update(prisoner);
                
                return RedirectToAction("Get");
            }

            return View(prisoner);
        }
        
        [HttpGet]
        public IActionResult Delete()
        {
            return View();
        }
    }
}