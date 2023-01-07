using DemoApplication.Areas.Admin.ViewModels.Subscribers;
using DemoApplication.Database;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/subscribers")]
    [Authorize(Roles = "admin")]
    public class SubscribersController : Controller
    {
        private readonly DataContext _dataContext;

        public SubscribersController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("list", Name = "admin-subscribers-list")]
        public IActionResult List()
        {
            var model = _dataContext.Subscribes
                .Select(a => new SubscribersViewModel(a.Id,a.Email))
                .ToList();

            return View(model);
        }
    }
}
