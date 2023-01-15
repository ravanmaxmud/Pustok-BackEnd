using DemoApplication.Areas.Client.ViewModels.Home.Index;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Services.Abstracts;
using DemoApplication.Services.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace DemoApplication.Areas.Client.Controllers
{

    [Area("client")]
    [Route("shop")]
    public class ShopController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IFileService _fileService;

        public ShopController(DataContext dbContext, IFileService fileService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
        }

        [HttpGet("index", Name = "client-shop-index")]
        public async Task<IActionResult> Index(int sort, int page = 1)
        {
            var model = new List<BookListItemViewModel>();

            switch (sort)
            {
                case 1:
                    model = await _dbContext.Books.Skip((page - 1) * 4).Take(4).OrderBy(b => b.Title).Include(b => b.BookImages).Select
                        (b => new BookListItemViewModel(b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Price,
                        b.BookImages!.Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                        : String.Empty,
                        b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty))
                .ToListAsync();

                    break;
                case 2:
                    model = await _dbContext.Books.Skip((page - 1) * 4).Take(4).OrderByDescending(b => b.Title).Select
                        (b => new BookListItemViewModel(b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Price,
                        b.BookImages!.Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                        : String.Empty,
                        b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty))
                .ToListAsync();

                    break;
                case 3:
                    model = await _dbContext.Books.Skip((page - 1) * 4).Take(4).OrderBy(b => b.Price).Select
                        (b => new BookListItemViewModel(b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Price,
                        b.BookImages!.Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                        : String.Empty,
                        b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty))
                .ToListAsync();

                    break;
                case 4:
                    model = await _dbContext.Books.Skip((page - 1) * 4).Take(4).OrderByDescending(b => b.Price).Select
                       (b => new BookListItemViewModel(b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Price,
                        b.BookImages!.Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                        : String.Empty,
                        b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty))
                .ToListAsync();

                    break;
                default:
                    model = await _dbContext.Books.Skip((page - 1) * 4).Take(4).Select
                       (b => new BookListItemViewModel(b.Id,
                b.Title,
                $"{b.Author.FirstName} {b.Author.LastName}",
                        b.Price,
                        b.BookImages!.Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                        : String.Empty,
                        b.BookImages!.Skip(1).Take(1).FirstOrDefault()! != null
                ? _fileService.GetFileUrl(b.BookImages.Skip(1).Take(1).FirstOrDefault().ImageNameFileSystem, UploadDirectory.Book)
                : String.Empty))
                .ToListAsync();
                    break;
            }
            ViewBag.CurrentPage = page;
            ViewBag.TotalPage = Math.Ceiling((decimal)_dbContext.Books.Count() / 4);
            return View(model);
        }
    }
}
