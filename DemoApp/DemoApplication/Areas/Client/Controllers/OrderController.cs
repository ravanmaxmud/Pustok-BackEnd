using DemoApplication.Areas.Client.ActionFilter;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Areas.Client.ViewModels.Order;
using DemoApplication.Contracts.File;
using DemoApplication.Contracts.Order;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using DemoApplication.Services.Concretes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("Order")]
    public class OrderController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;

        private const int MIN_RANDOM_NUMBER = 10000;
        private const int MAX_RANDOM_NUMBER = 100000;
        private const string PREFIX = "OR";

        //private readonly IOrderService _orderService;
        public OrderController(DataContext dbContext, IFileService fileService, IUserService userService, IOrderService orderService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
            _userService = userService;
            _orderService = orderService;
        }

        [HttpGet("checkout",Name ="client-order-checkout")]
        [ServiceFilter(typeof(ValidationCurrentUserAttribute))]
        public async Task<IActionResult> CheckOut()
        {
            var model = new OrderViewModel
            {
                SumTotal = _dbContext.BasketProducts.Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id)
                .Sum(bp => bp.Book.Price * bp.Quantity),
                ProductList = await _dbContext.BasketProducts.Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id)
                .Select(bp => new OrderViewModel.ListItemViewModel(
                    bp.Id,
                    bp.Book.Title,
                    bp.Quantity,
                    bp.Book.Price,
                    bp.Book.Price * bp.Quantity)).ToListAsync()
            };
            return View(model);
        }

        [HttpPost("placeorder/{userId}", Name = "client-order-placeorder")]
        public async Task<IActionResult> PlaceOrder([FromRoute] int userId)
        {
            var user = await _dbContext.Users.FirstOrDefaultAsync(b => b.Id == userId);

            var model = new OrderViewModel
            {
                SumTotal = _dbContext.BasketProducts.Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id)
                .Sum(bp => bp.Book.Price * bp.Quantity),
                ProductList = await _dbContext.BasketProducts.Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id)
                .Select(bp => new OrderViewModel.ListItemViewModel(
                    bp.Id,
                    bp.Book.Title,
                    bp.Quantity,
                    bp.Book.Price,
                    bp.Book.Price * bp.Quantity)).ToListAsync()
            };





            var order = await _orderService.AddOrderAsync(model, model.SumTotal);



            await _orderService.AddOrderProductAsync(model);

            var pasketProducts = await _dbContext.BasketProducts
                        .Where(bp => bp.Basket.UserId == _userService.CurrentUser.Id)
                       .ToListAsync();

            _dbContext.BasketProducts.RemoveRange(pasketProducts);


            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("client-account-orders");



           
        }


    }
}
