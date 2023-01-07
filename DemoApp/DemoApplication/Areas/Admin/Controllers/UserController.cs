using DemoApplication.Areas.Admin.ViewModels.Author;
using DemoApplication.Areas.Admin.ViewModels.Book.Add;
using DemoApplication.Areas.Admin.ViewModels.Role;
using DemoApplication.Areas.Admin.ViewModels.User;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Migrations;
using DemoApplication.Services.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Basket = DemoApplication.Database.Models.Basket;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/user")]
    [Authorize(Roles = "admin")]
    public class UserController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<BookController> _logger;
        private readonly IUserService _userService;

        public UserController(DataContext dataContext, ILogger<BookController> logger, IUserService userService)
        {
            _dataContext = dataContext;
            _logger = logger;
            _userService = userService;
        }

        [HttpGet("list", Name = "admin-user-list")]
        public async Task<IActionResult> List()
        {
            var model = await _dataContext.Users.Select(u =>
            new UserListItemViewModel(u.Id,u.Email, u.FirstName, u.LastName, u.Roles.Name
                )).ToListAsync();
            return View(model);
        }

        [HttpGet("add", Name = "admin-user-add")]
        public async Task<IActionResult> Add()
        {
            var model = new UserAddViewModel
            {
                Roles = await _dataContext.Roles.Select(a => new RoleViewModel(a.Id, a.Name))
                    .ToListAsync(),
            };
            return View(model);
        }

        [HttpPost("add", Name = "admin-user-add")]
        public async Task<IActionResult> Add(UserAddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model = new UserAddViewModel
                {
                    Roles = await _dataContext.Roles.Select(a => new RoleViewModel(a.Id, a.Name))
      .ToListAsync(),
                };
                return View(model);
            }

            if (!_dataContext.Roles.Any(a => a.Id == model.RoleId))
            {
                model = new UserAddViewModel
                {
                    Roles = await _dataContext.Roles.Select(a => new RoleViewModel(a.Id, a.Name))
      .ToListAsync(),
                };
                ModelState.AddModelError(string.Empty, "Role is not found");
                return View(model);
            }

            var user = new User
            {
                RoleId = model.RoleId,
                Email = model.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(model.Password),
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _dataContext.Users.AddAsync(user);

            var basket = new Basket
            {
                User = user,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-user-list");
        }


        [HttpGet("update/{id}", Name = "admin-user-update")]
        public async Task<IActionResult> Update([FromRoute]int id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(u => u.Id == id);
            if (user == null)
            {
                return NotFound();
            }
            var model = new UserUpdateViewModel
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                RoleId = user.RoleId,
                Roles = await _dataContext.Roles.Select(r => new RoleViewModel(r.Id, r.Name)).ToListAsync()
            };
            return View(model);
        }
        [HttpPost("update/{id}", Name = "admin-user-update")]
        public async Task<IActionResult> Update([FromRoute] int id,[FromForm]UserUpdateViewModel model)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(b => b.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                ModelState.AddModelError(string.Empty, "Role is not found");
                return View(model);
            }

            //if (!_dataContext.Roles.Any(a => a.Id == model.RoleId))
            //{
            //    ModelState.AddModelError(string.Empty, "Role is not found");
            //    return View(model);
            //}

            user.Email = model.Email;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.RoleId = model.RoleId;
            user.CreatedAt = DateTime.Now;

            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-user-list");
        }

        [HttpPost("delete/{id}", Name = "admin-user-delete")]
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var user = await _dataContext.Users.FirstOrDefaultAsync(b => b.Id == id);
            if (user is null)
            {
                return NotFound();
            }

            _dataContext.Users.Remove(user);
            await _dataContext.SaveChangesAsync();

            return RedirectToRoute("admin-user-list");
        }
    }
}
