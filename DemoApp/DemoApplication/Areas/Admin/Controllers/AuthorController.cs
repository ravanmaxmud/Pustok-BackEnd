using DemoApplication.Areas.Admin.ViewModels.Author;
using DemoApplication.Database.Models;
using Microsoft.AspNetCore.Mvc;
using DemoApplication.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Components;
using RouteAttribute = Microsoft.AspNetCore.Components.RouteAttribute;
using Nancy.Json;
using DemoApplication.Areas.Admin.ViewCompanents;
using DemoApplication.Areas.Admin.ViewModels.Book.Add;
using Nancy;
using Microsoft.AspNetCore.Authorization;

namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/author")]
    [Authorize(Roles = "admin")]
    public class AuthorController : Controller
    {
        private readonly DataContext _dataContext;

        public AuthorController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        [HttpGet("list", Name = "admin-author-list")]
        public async Task<IActionResult> List()
        {
            var model = await _dataContext.Authors
                .Select(a => new ListItemViewModel(a.Id, a.FirstName, a.LastName))
                .ToListAsync();
            return View(model);
        }

        [HttpPost("add", Name = "admin-author-Add")]
        public async Task<IActionResult> Add(Admin.ViewModels.Author.AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var addModel = ViewComponent(nameof(AddModal),model);
                addModel.StatusCode = (int)HttpStatusCode.BadRequest;

                return addModel;
            }
            var author = new Author {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
            };

             _dataContext.Authors.Add(author);

            await _dataContext.SaveChangesAsync();

            var responseModel = new ListItemViewModel(author.Id,author.FirstName,author.LastName);

            var listItemPartialView = PartialView("Partials/Author/_ListItem", responseModel);
              listItemPartialView.StatusCode = (int)HttpStatusCode.Created;

            return listItemPartialView;
        }



        [HttpGet("update/{id}", Name = "admin-author-update")]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var author = await _dataContext.Authors.FirstOrDefaultAsync(x => x.Id == id);

            if (author == null)
            {
                return NotFound();
            }

            var model = new UpdateViewModel {
                FirstName = author.FirstName,
                LastName= author.LastName,
            };

            return ViewComponent(nameof(UpdateModalAuthor),model);
        }



        [HttpDelete("delete", Name = "admin-author-delete")]
        public async Task<IActionResult> Delete(int id)
        {
            var author = await _dataContext.Authors.FirstOrDefaultAsync(a=> a.Id == id);

            if (author is null)
            {
                return NotFound();
            }

            _dataContext.Authors.Remove(author);
            await _dataContext.SaveChangesAsync();

            return NoContent();
        }


    }
}
