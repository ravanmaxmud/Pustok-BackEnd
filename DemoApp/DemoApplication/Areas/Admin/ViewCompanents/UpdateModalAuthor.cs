using DemoApplication.Areas.Admin.ViewModels.Author;
using Microsoft.AspNetCore.Mvc;

namespace DemoApplication.Areas.Admin.ViewCompanents
{
    public class UpdateModalAuthor : ViewComponent
    {
        public IViewComponentResult Invoke(UpdateViewModel? model = null)
        {
            return View(model ?? new UpdateViewModel());
        }

    }
}
