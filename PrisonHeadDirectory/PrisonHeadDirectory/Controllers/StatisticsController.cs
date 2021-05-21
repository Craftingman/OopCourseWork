using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;

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
            return View();
        }
    }
}