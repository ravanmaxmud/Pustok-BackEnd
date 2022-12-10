using DemoApplication.Areas.Client.ViewModels.Colors;
using DemoApplication.Database;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("Color")]
    public class ColorController1 : Controller
    {
        private readonly DataContext _dataContext;

        public ColorController1(DataContext dbContext)
        {
            _dataContext = dbContext;
        }


        [HttpGet("add/{id}", Name = "client-color-add")]
        public async Task<IActionResult> AddColorAsync([FromRoute] int id)
        {
            var color = await _dataContext.Colors.FirstOrDefaultAsync(c=> c.Id == id);

            var colorCookieVal = HttpContext.Request.Cookies["Color"];

            if (colorCookieVal is null)
            {
                var colorsCookieViewModel = new ColorViewModel(color.Id,color.Name);

                HttpContext.Response.Cookies.Append("Color", JsonSerializer.Serialize(colorsCookieViewModel));
            }
            else
            {
                var colorsCookieViewModel = JsonSerializer.Deserialize<ColorViewModel>(colorCookieVal);

                colorsCookieViewModel = new ColorViewModel(color.Id, color.Name);

                HttpContext.Response.Cookies.Append("Color", JsonSerializer.Serialize(colorsCookieViewModel));
            }

            return RedirectToRoute("client-home-index");
        }
    }
}
