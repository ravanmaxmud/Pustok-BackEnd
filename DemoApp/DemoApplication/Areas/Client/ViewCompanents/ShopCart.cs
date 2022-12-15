using DemoApplication.Areas.Client.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;

namespace DemoApplication.Areas.Client.ViewCompanents
{
    public class ShopCart : ViewComponent
    {
        public IViewComponentResult Invoke(List<ProductCookieViewModel>? models = null)
        {
            var productsCookieValue = HttpContext.Request.Cookies["products"];

            var productsCookieViewModel = new List<ProductCookieViewModel>();

            if (productsCookieValue is not null)
            {
                productsCookieViewModel = JsonSerializer.Deserialize<List<ProductCookieViewModel>>(productsCookieValue);
            }

            if (models is not null)
            {
                productsCookieViewModel = models;
            }

            return View(productsCookieViewModel);
        }
    }
}
