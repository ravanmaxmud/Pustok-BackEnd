using DemoApplication.Areas.Client.ViewModels.Home.Index;
using DemoApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{

    [Area("client")]
    [Route("shop")]
    public class ShopController : Controller
    {
        private readonly DataContext _dbContext;

        public ShopController(DataContext dbContext)
        {
            _dbContext = dbContext;
        }

        [HttpGet("index", Name = "client-shop-index")]
        public async Task<IActionResult> Index(int sort)
        {
            var model = new IndexViewModel();
           
            switch (sort)
            {
                case 1:
                    model = new IndexViewModel
                    {
                        Books = await _dbContext.Books.Select
                       (b => new BookListItemViewModel(b.Id, b.Title, $"{b.Author.FirstName} {b.Author.LastName}", b.Price))
                       .ToListAsync()
                    };
                    break;
                default:
                    model = new IndexViewModel
                    {
                        Books = await _dbContext.Books.Select
                        (b => new BookListItemViewModel(b.Id, b.Title, $"{b.Author.FirstName} {b.Author.LastName}", b.Price))
                        .ToListAsync()
                    };
                    break;
            }
            return View(model);
        }
    }
}
