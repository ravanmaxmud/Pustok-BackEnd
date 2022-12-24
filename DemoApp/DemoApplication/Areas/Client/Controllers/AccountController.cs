using DemoApplication.Areas.Client.ViewModels.Account;
using DemoApplication.Database;
using DemoApplication.Database.Models;
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
        public async Task<IActionResult> Address()
        {
            var user = _userService.CurrentUser;

            var address = await _dataContext.Address.FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (address is null)
            {
                return RedirectToRoute("client-account-edit-address",new EditAddressViewModel());
            }

            var model = new AddressListViewModel 
            {
               User = $"{address.User.FirstName} {address.User.LastName}",
               PhoneNumber = address.PhoneNum,
               Title = address.Title
            };
            return View(model);
        }
        [HttpGet("editAddress", Name = "client-account-edit-address")]
        public async Task<IActionResult> EditAddress()
        {
            var user = _userService.CurrentUser;

            var address = await _dataContext.Address.FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (address is null)
            {
                return View(new EditAddressViewModel());
            }


            var model = new EditAddressViewModel
            {
                PhoneNumber = address.PhoneNum,
                Title = address.Title
            };
            return View(model);
        }

        [HttpPost("editAddress", Name = "client-account-edit-address")]
        public async Task<IActionResult> EditAddress(EditAddressViewModel model)
        {
            var user = _userService.CurrentUser;

            var address = await _dataContext.Address.FirstOrDefaultAsync(a => a.UserId == user.Id);

            if (address is not null)
            {
              address.PhoneNum = model.PhoneNumber;
                address.Title = model.Title;

            }
            else
            {
                var newAddress = new Addres 
                {
                  UserId=user.Id,
                  PhoneNum = model.PhoneNumber,
                  Title = model.Title
                };
                await _dataContext.Address.AddAsync(newAddress);
            }

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("client-account-address");
        }

        //[HttpPost("address", Name = "client-account-address")]
        //public IActionResult Address()
        //{
        //    var user = _userService.CurrentUser;

        //    return View();
        //}

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
