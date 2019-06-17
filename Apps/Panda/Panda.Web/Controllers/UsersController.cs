﻿using Panda.Services;
using Panda.Web.ViewModels.Users;
using SIS.MvcFramework;
using SIS.MvcFramework.Attributes;
using SIS.MvcFramework.Attributes.Security;
using SIS.MvcFramework.Result;

namespace Panda.Web.Controllers
{
    public class UsersController : Controller
    {

        private readonly IUserService userService;

        public UsersController(IUserService userService)
        {
            this.userService = userService;
        }

        public IActionResult Login()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Login(UserLoginInputModel model)
        {

            if (!this.ModelState.IsValid)
            {

            }


            var existingUser = this.userService.GetUserOrNull(model.Username, model.Password);

            if (existingUser == null)
            {
                return this.Redirect("/Users/Login");
            }

            this.SignIn(existingUser.Id, existingUser.Username, existingUser.Email);

            return this.Redirect("/Home/Index");
        }

        public IActionResult Register()
        {
            return this.View();
        }

        [HttpPost]
        public IActionResult Register(UserRegisterInputModel model)
        {

            if (!this.ModelState.IsValid)
            {
                return this.Redirect("/Users/Register");
            }

            if (model.Password != model.ConfirmPassword)
            {
                return this.Redirect("/Users/Register");
            }

            string userId = this.userService.CreateUser(model.Username, model.Email, model.Password);
            this.SignIn(userId, model.Username, model.Email);

            return this.Redirect("/Home/Index");

        }

        [Authorize]
        public IActionResult Logout()
        {
            this.SignOut();

            return Redirect("/Home/Index");
        }
    }
}