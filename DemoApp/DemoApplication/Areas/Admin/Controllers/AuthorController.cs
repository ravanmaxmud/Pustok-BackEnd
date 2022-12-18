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


namespace DemoApplication.Areas.Admin.Controllers
{
    [Area("admin")]
    [Route("admin/author")]
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

        [HttpPost("add", Name = "admin-author-add")]
        public async Task<IActionResult> Add(ViewModels.Author.AddViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var author = new Author {
                FirstName = model.FirstName,
                LastName = model.LastName,
                CreatedAt = DateTime.Now

            };

            await _dataContext.Authors.AddAsync(author);
       
            await _dataContext.SaveChangesAsync();



            var id = author.Id;

            return Created("admin-author-add", id);

        }


        [HttpDelete("delete/{id}", Name = "admin-author-delete")]
        public async Task<IActionResult> DeleteAsync([FromRoute] int id)
        {
            var author = await _dataContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
            if (author is null)
            {
                return NotFound();
            }

            _dataContext.Authors.Remove(author);
            await _dataContext.SaveChangesAsync();

            return NoContent();
            
        }



        [HttpGet("update/{id}", Name = "admin-author-update")]
        public async Task<IActionResult> Update([FromRoute] int id)
        {
            var author = await _dataContext.Authors.FirstOrDefaultAsync(b => b.Id == id);
            if (author is null)
            {
                return NotFound();
            }

            var model = new UpdateViewModel {
               FirstName = author.FirstName,
               LastName= author.LastName
            };

            return View(model);
        }

        [HttpPut("update/{id}", Name = "admin-author-update")]
        public async Task<IActionResult> Update(UpdateViewModel model)
        {
            var author = await _dataContext.Authors.FirstOrDefaultAsync(a => a.Id == model.Id);


            if (author is null)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            author.FirstName = model.FirstName;
            author.LastName = model.LastName;
            author.UpdatedAt = DateTime.Now;

           
            await _dataContext.SaveChangesAsync();

            return StatusCode(200);
        }



    }
}
