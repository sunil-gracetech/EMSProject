using EMSProject.Models.ViewModel;
using EMSProject.Respository.Contract;
using EMSProject.Utils.Enums;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace EMSProject.Controllers
{
    public class AccountController : Controller
    {
        private IUser userService;
        private readonly IHostEnvironment hostingEnvironment;

        public AccountController(IUser user, IHostEnvironment hostingEnvironment) {
            userService = user;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Login()
        {
            return View();
        } 
        [HttpPost]
        public IActionResult Login(SignIn model)
        {
            if (ModelState.IsValid)
            {
                var result = userService.AuthenticateUser(model);
                if (result == AuthoEnum.SUCCESS)
                {
                    //Create the identity for the user  
                    var identity = new ClaimsIdentity(new[] {
                    new Claim(ClaimTypes.Name, model.Email)
                }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var principal = new ClaimsPrincipal(identity);

                    var login = HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);


                    return RedirectToAction("Index", "Home");
                }
                else if(result==AuthoEnum.FAILED)
                {
                    ModelState.AddModelError(string.Empty, "Invalid login credentails !");
                    return View();
                }else if (result == AuthoEnum.NOTVERIFIED)
                {
                    ModelState.AddModelError(string.Empty, "Your account is still not active, please verify your account !");
                    return View();
                }
                ModelState.AddModelError(string.Empty, "You are not a valid user !");
                return View();
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please enter login details !");
                return View();
            }
        }

        public IActionResult Logout()
        {
            var login = HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        public IActionResult UpdateProfile()
        {
            return View();
        }

        public string UploadProfile(IFormFile file)
        {
          var www= hostingEnvironment.ContentRootPath;
            var fullpath = Path.Combine(www,"images",file.FileName);

            FileStream stream = new FileStream(fullpath, FileMode.Create);         
            file.CopyTo(stream);
            return $"images/{file.FileName}";
        }
        [HttpPost]
        public IActionResult UpdateProfile(string Email)
        {
            var file = Request.Form.Files;
            if (file.Count > 0)
            {
              string path=UploadProfile(file[0]);
                userService.UpdateProfile(Email, path);
                return RedirectToAction("Index", "Home");
            }
            else{
                ModelState.AddModelError(string.Empty, "Please select file !");
            }
            return View();
        }

        public IActionResult Register()
        {
            return View();
        }
        [HttpPost]
        public IActionResult Register(SignUp model)
        {
            if (ModelState.IsValid)
            {
                var result=userService.Register(model);
                if (result != null)
                {
                   return RedirectToAction("VerifyUser");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email Already exist !");
                    return View(model);
                }
            }


            return View(model);
        }

        public IActionResult VerifyUser()
        {
            return View();
        }

        [HttpPost]
        public IActionResult VerifyUser(string Otp)
        {
            if (Otp != null)
            {
                VerifyAccountEnum result = userService.VerifyAccount(Otp);
                if (result == VerifyAccountEnum.OTPVERIFIED)
                {

                    return RedirectToAction("Login");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid OTP !");
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Please enter OTP");
                return View();
            }
        }

    }
}
