using DemoApplication.Areas.Client.ViewModels.Authentication;
using DemoApplication.Areas.Client.ActionFilter;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XAct.Users;
using Microsoft.EntityFrameworkCore;
using DemoApplication.Contracts.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using DemoApplication.Contracts.Email;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("authentication")]
    public class AuthenticationController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IUserService _userService;
        private readonly IEmailService _emailService;
        public AuthenticationController(DataContext dataContext , IUserService userService,IEmailService emailService)
        {
            _dbContext = dataContext;
            _userService = userService;
            _emailService = emailService;
        }

   
        [HttpGet("login",Name ="client-auth-login")]
        [ServiceFilter(typeof(ValidationCurrentUserAttribute))]
        public async Task<IActionResult> Login()
        {
            return View();
        }
        [HttpPost("login", Name = "client-auth-login")]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (!await _userService.CheckPasswordAsync(model!.Email, model!.Password))
            {
                ModelState.AddModelError(String.Empty, "Email or password is not correct");
                return View(model);
            }

            if (await _dbContext.Users.AnyAsync(u=> u.Email == model.Email && u.Roles.Name ==RoleNames.ADMIN))
            {
                await _userService.SignInAsync(model!.Email, model!.Password,RoleNames.ADMIN);
                return RedirectToRoute("admin-auth-login");
            }

            await _userService.SignInAsync(model!.Email, model!.Password);

            return RedirectToRoute("client-home-index");
        }

        [HttpGet("logout", Name = "client-auth-logout")]
        public async Task<IActionResult> Logout()
        {
            await _userService.SignOutAsync();

            return RedirectToRoute("client-home-index");
        }


        [HttpGet("register", Name = "client-auth-register")]
        [ServiceFilter(typeof(ValidationCurrentUserAttribute))]
        public IActionResult Register()
        { 
            return View();
        }

        [HttpPost("register", Name = "client-auth-register")]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var emails = new List<string>();

            emails.Add(model.Email);


            await _userService.CreateAsync(model);
           
            return RedirectToRoute("client-home-index");
        }

        [HttpGet("activate/{token}", Name = "client-auth-activate")]
        public async Task<IActionResult> ActivateAsync([FromRoute] string token)
        {
            var userActivation = await _dbContext.UserActivations
                .Include(ua => ua.User)
                .FirstOrDefaultAsync(ua =>
                    !ua!.User!.IsActive &&
                    ua.ActivationToken == token);

            if (userActivation is null)
            {
                return NotFound();
            }

            if (DateTime.Now > userActivation!.ExpiredDate)
            {
                return Ok("Token expired olub teessufler");
            }

            userActivation!.User!.IsActive = true;

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("client-auth-login");
        }
    }
}
