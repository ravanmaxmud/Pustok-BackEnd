using DemoApplication.Areas.Admin.ViewModels.Author;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Areas.Admin.ViewCompanents
{
    public class ListAuthView : ViewComponent
    {
        public IViewComponentResult Invoke(AddViewModel model = null)
        {
       
            return View(model ?? new AddViewModel());
        }
    }
}
