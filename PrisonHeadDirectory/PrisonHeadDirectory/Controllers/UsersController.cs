using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Core;
using DAL.Abstractions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PrisonHeadDirectory.Models;

namespace PrisonHeadDirectory.Controllers
{
    public class UsersController : Controller
    {
        private readonly IUserDalService _userDalService;
        
        public UsersController(IUserDalService userDalService)
        {
            _userDalService = userDalService;
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Get(string searchStr = "")
        {
            IEnumerable<User> users = _userDalService.GetUsers();
            return View(users);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Edit(int id)
        {
            User user = _userDalService.GetUser(id);
            
            SelectList roles = new SelectList(_userDalService.GetRoles(), 
                "Name", "Name");
            ViewBag.Roles = roles;
            
            EditUserViewModel editViewModel = new EditUserViewModel()
            {
                Id = id,
                Name = user.Name,
                MiddleName = user.MiddleName,
                Email = user.Email,
                RoleName = user.Role.Name,
                Surname = user.Surname
            };
            return View(editViewModel);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Edit(EditUserViewModel editViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Id = editViewModel.Id,
                    Name = editViewModel.Name,
                    Surname = editViewModel.Surname,
                    MiddleName = editViewModel.MiddleName,
                    Email = editViewModel.Email
                };
                
                _userDalService.UpdateUser(user, editViewModel.RoleName);

                return RedirectToAction("Get");
            }

            return View(editViewModel);
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            User user = _userDalService.GetUser(id);
            return View(user);
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Delete(User user)
        {
            _userDalService.DeleteUser(user.Id);
            return RedirectToAction("Get");
        }
        
        [Authorize(Roles = "admin")]
        [HttpGet]
        public IActionResult Create()
        {
            SelectList roles = new SelectList(_userDalService.GetRoles(), 
                "Name", "Name");
            ViewBag.Roles = roles;
            return View();
        }
        
        [Authorize(Roles = "admin")]
        [HttpPost]
        public IActionResult Create(RegisterUserViewModel registerViewModel)
        {
            if (ModelState.IsValid)
            {
                User user = new User()
                {
                    Name = registerViewModel.Name,
                    Surname = registerViewModel.Surname,
                    MiddleName = registerViewModel.MiddleName,
                    Email = registerViewModel.Email,
                    Password = registerViewModel.Password
                };
            
                _userDalService.CreateUser(user, registerViewModel.RoleName);
                return RedirectToAction("Get");
            }

            return View(registerViewModel);
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = _userDalService.GetUserForAuth(model.Email, model.Password);
                if (user != null)
                {
                    await Authenticate(user);
 
                    return RedirectToAction("Index", "Home");
                }
                ModelState.AddModelError("", "Некорректные логин и(или) пароль");
            }
            
            return View(model);
        }
        
        private async Task Authenticate(User user)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, user.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role?.Name)
            };
            
            ClaimsIdentity id = new ClaimsIdentity(
                claims, "ApplicationCookie", 
                ClaimsIdentity.DefaultNameClaimType, 
                ClaimsIdentity.DefaultRoleClaimType);
            
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(id));
        }
        
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login", "Users");
        }
        
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            bool result = _userDalService.UserExists(email.ToLower());
                
            return Json(!result);
        }
    }
}