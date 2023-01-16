using DemoApplication.Areas.Client.ViewCompanents;
using DemoApplication.Areas.Client.ViewComponents;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Areas.Client.ViewModels.Home.Index;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Services.Abstracts;
using DemoApplication.Services.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("ShopCartBasket")]
    public class ShopCartBasketController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IUserService _userService;
        public ShopCartBasketController(DataContext dbContext, IUserService userService)
        {
            _dbContext = dbContext;
            _userService = userService;
        }


        [HttpGet("index",Name ="client-shop-cart-index")]
        public async Task<IActionResult> Index([FromServices] IFileService fileService)
        {
            var model = new IndexViewModel
            {

                Books = await _dbContext.Books
                .Select(b => new BookListItemViewModel
                (b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                b.Price,
                b.BookImages!.Take(1).FirstOrDefault()! != null
                ? fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty,
                b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty)).ToListAsync()
            };

            return View(model);
        }



        [HttpGet("shop-basket-delete/{id}", Name = "client-shop-basket-delete")]
        public async Task<IActionResult> DeleteBaketProductAsync([FromRoute] int id)
        {


            if (_userService.IsAuthenticated)
            {

                var basketProduct = await _dbContext.BasketProducts
                        .FirstOrDefaultAsync(bp => bp.Basket.UserId == _userService.CurrentUser.Id && bp.BookId == id);

                if (basketProduct is null) return NotFound();

                _dbContext.BasketProducts.Remove(basketProduct);
            }
            else
            {

                var product = await _dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
                if (product is null)
                {
                    return NotFound();
                }

                var productCookieValue = HttpContext.Request.Cookies["products"];
                if (productCookieValue is null)
                {
                    return NotFound();
                }

                var productsCookieViewModel = JsonSerializer.Deserialize<List<ProductCookieViewModel>>(productCookieValue);
                productsCookieViewModel!.RemoveAll(pcvm => pcvm.Id == id);

                HttpContext.Response.Cookies.Append("products", JsonSerializer.Serialize(productsCookieViewModel));
            }


            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("client-shop-cart-index");
        }
    }
}
