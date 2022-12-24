using DemoApplication.Areas.Admin.ViewModels.Author;
using DemoApplication.Areas.Client.ViewModels.Basket;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Areas.Admin.ViewCompanents
{
    public class AddModal : ViewComponent
    {
        public IViewComponentResult Invoke(AddViewModel? model = null)
        {
            return View(model ?? new AddViewModel());
        }
    }
}
