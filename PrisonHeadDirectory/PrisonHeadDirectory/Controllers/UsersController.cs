using DAL.Abstractions;
using Microsoft.AspNetCore.Mvc;

namespace PrisonHeadDirectory.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserDalService _userDalService;
        
        public UsersController(IUserDalService userDalService)
        {
            _userDalService = userDalService;
        }

        [HttpGet]
        public IActionResult Get()
        { 
            return View();
        }
        
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            bool result = _userDalService.UserExists(email.ToLower());
                
            return Json(!result);
        }
    }
}