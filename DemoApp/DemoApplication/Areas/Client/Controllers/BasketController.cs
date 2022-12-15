using DemoApplication.Areas.Client.ViewCompanents;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using System.Xml;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("basket")]
    public class BasketController : Controller
    {
        private readonly DataContext _dataContext;

        public BasketController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("basket/{id}", Name = "client-basket-add")]
        public async Task<IActionResult> AddProductAsync([FromRoute] int id)
        {
            var products = await _dataContext.Books.FirstOrDefaultAsync(p => p.Id == id);
            if (products is null)
            {
                return NotFound();
            }
        
            var productsCookieValue = HttpContext.Request.Cookies["products"];

            var productViewModel = new List<ProductCookieViewModel>();


            if (productsCookieValue is null)
            {
                 productViewModel = new List<ProductCookieViewModel>()
                {
                   new ProductCookieViewModel(products.Id,products.Title,String.Empty,1,products.Price,products.Price)
                };

                HttpContext.Response.Cookies.Append("products", JsonSerializer.Serialize(productViewModel));
            }
            else
            {
                productViewModel = JsonSerializer.Deserialize<List<ProductCookieViewModel>>(productsCookieValue);
                                                                

                var cookieViewModel = productViewModel!.FirstOrDefault(c => c.Id == id);

                if (cookieViewModel is null)
                {
                    productViewModel!.Add(new ProductCookieViewModel(products.Id, products.Title, String.Empty, 1, products.Price, products.Price));
                }
                else
                {
                    cookieViewModel.Quantity += 1;
                    cookieViewModel.Total = cookieViewModel.Quantity * cookieViewModel.Price;

                }
                HttpContext.Response.Cookies.Append("products", JsonSerializer.Serialize(productViewModel));

            }

            //return RedirectToRoute("client-home-index");
            return ViewComponent(nameof(ShopCart), productViewModel);
        }



        [HttpGet("basket-delete/{id}", Name = "client-basket-delete")]
        public async Task<IActionResult> DeleteProductAsync([FromRoute] int id)
        {

            var products = await _dataContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            if (products is null)
            {
                return NotFound();
            }
            var producktCookieViewModel = new List<ProductCookieViewModel>();

            var productCookieValue = HttpContext.Request.Cookies["products"];


            if (productCookieValue is null)
            {
                return NotFound();
            }

             producktCookieViewModel = JsonSerializer.Deserialize
                                             <List<ProductCookieViewModel>>(productCookieValue);


            producktCookieViewModel!.RemoveAll(b => b.Id == id);


            HttpContext.Response.Cookies.Append("products", JsonSerializer.Serialize(producktCookieViewModel));

            return ViewComponent(nameof(ShopCart), producktCookieViewModel);
        }

    }
}
