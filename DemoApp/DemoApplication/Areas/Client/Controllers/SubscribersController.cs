using DemoApplication.Areas.Client.ViewModels.Subscribers;
using DemoApplication.Database;
using DemoApplication.Migrations;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("Subscribers")]
    public class SubscribersController : Controller
    {
        private readonly DataContext _dataContext;

        public SubscribersController(DataContext dbContext)
        {
            _dataContext = dbContext;
        }

        [HttpPost("subAdd", Name = "client-subscribers-add")]
        public async Task<IActionResult> AddSubAsync(SubscribersViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            if (_dataContext.Subscribes.Any(s=> s.Email == model.Email))
            {
                ModelState.AddModelError(string.Empty, "Email is Used");
                return BadRequest("NotFound");
            }
        
            _dataContext.Subscribes.Add(new Database.Models.Subscribes
            {
              Email = model.Email
            });

            _dataContext.SaveChanges();

            return Ok();
        }
    }
}
