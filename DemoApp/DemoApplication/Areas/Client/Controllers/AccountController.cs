using DemoApplication.Areas.Client.ViewModels.Account;
using DemoApplication.Database;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("account")]
    [Authorize]
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;

        public AccountController(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
        }

        [HttpGet("dashboard", Name = "client-account-dashboard")]
        public IActionResult Dashboard()
        {
            var user = _userService.CurrentUser;

            return View();
        }

        [HttpGet("orders", Name = "client-account-orders")]
        public IActionResult Orders()
        {
            var user = _userService.CurrentUser;

            return View();
        }
        [HttpGet("download", Name = "client-account-download")]
        public IActionResult Download()
        {
            var user = _userService.CurrentUser;

            return View();
        }

        [HttpGet("address", Name = "client-account-address")]
        public IActionResult Address()
        {
            var user = _userService.CurrentUser;

            return View();
        }

        [HttpGet("details", Name = "client-account-details")]
        public async Task<IActionResult> Details()
        {

            var currentUser = _userService.CurrentUser;

            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            var model = new UserViewModel
            {
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
            };

            return View(model);
        }

        [HttpPost("details", Name = "client-account-details")]
        public async Task<IActionResult> Details(UserViewModel model)
        {

            var currentUser = _userService.CurrentUser;
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);

            if (user == null) return View(model);
            if (!ModelState.IsValid)
            {
                return NotFound();
            }

            user.FirstName = model.FirstName;
            user.LastName = model.LastName;

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-account-details"); 
        }


        [HttpPost("detailsPassword", Name = "client-account-details-password")]
        public async Task<IActionResult> PasswordDetails(UserPasswordViewModel model)
        {

            var currentUser = _userService.CurrentUser;
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == currentUser.Id);


            bool verified = BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.Password);

            if (verified == true)
            {

                string passwordHash = BCrypt.Net.BCrypt.HashPassword(model.NewPassword);

                user.Password = passwordHash;
            }
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-account-details");
        }
    }
}
