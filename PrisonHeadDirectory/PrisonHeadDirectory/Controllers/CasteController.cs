using System.Collections;
using System.Collections.Generic;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace PrisonHeadDirectory.Controllers
{
    public class CasteController : Controller
    {
        private readonly ICasteDalService _casteDalService;

        public CasteController(ICasteDalService casteDalService)
        {
            _casteDalService = casteDalService;
        }
        
        [HttpGet]
        public IActionResult Get()
        {
            IEnumerable<Caste> castes = _casteDalService.GetAll();
            return View(castes);
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            _casteDalService.Delete(id);
            return RedirectToAction("Get");
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Caste caste)
        {
            if (ModelState.IsValid)
            {
                _casteDalService.Add(caste);

                return RedirectToAction("Get");
            }

            return View(caste);
        }
        
        public string GetCasteName(int id)
        {
            Caste caste = _casteDalService.Get(id);
            if (caste is null)
            {
                return "-- не выбрано --";
            }
            
            return caste.Name;
        }
    }
}