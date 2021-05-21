using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace PrisonHeadDirectory.Controllers
{
    public class RelativeController : Controller
    {
        private readonly IRelativeDalService _relativeDalService;

        public RelativeController(IRelativeDalService relativeDalService)
        {
            _relativeDalService = relativeDalService;
        }

        [HttpGet]
        public IActionResult Create(int prisonerId)
        {
            ViewBag.PrisonerId = prisonerId;
            return View(new Relative());
        }
        
        [HttpPost]
        public IActionResult Create(Relative relative)
        {
            if (ModelState.IsValid)
            {
                _relativeDalService.Add(relative);

                return RedirectToAction("Edit", "Prisoner", new {id = relative.PrisonerId});
            }

            return View(relative);
        }
    }
}