using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using DAL.Abstractions;
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

        [HttpGet]
        public IActionResult Get(string searchStr = "")
        {
            IEnumerable<User> users = _userDalService.GetUsers();
            return View(users);
        }
        
        [HttpGet]
        public IActionResult Edit(int id)
        {
            User user = _userDalService.GetUser(id);
            return View(user);
        }
        
        [HttpGet]
        public IActionResult Delete(int id)
        {
            User user = _userDalService.GetUser(id);
            return View(user);
        }
        
        [HttpPost]
        public IActionResult Delete(User user)
        {
            return View(user);
        }
        
        [HttpGet]
        public IActionResult Create()
        {
            SelectList roles = new SelectList(_userDalService.GetRoles(), 
                "Name", "Name");
            ViewBag.Roles = roles;
            return View();
        }
        
        [HttpPost]
        public IActionResult Create(RegisterViewModel registerViewModel)
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
        
        [AcceptVerbs("Get", "Post")]
        public IActionResult CheckEmail(string email)
        {
            bool result = _userDalService.UserExists(email.ToLower());
                
            return Json(!result);
        }
    }
}