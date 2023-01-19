using DemoApplication.Areas.Client.ActionFilter;
using DemoApplication.Areas.Client.ViewModels.Basket;
using DemoApplication.Areas.Client.ViewModels.OrderProducts;
using DemoApplication.Contracts.File;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using DemoApplication.Services.Concretes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DemoApplication.Areas.Client.Controllers
{
    [Area("client")]
    [Route("Order")]
    [Authorize]
    public class OrderController : Controller
    {
        private readonly DataContext _dbContext;
        private readonly IFileService _fileService;
        private readonly IUserService _userService;
        private readonly IOrderService _orderService;
        public OrderController(DataContext dbContext, IFileService fileService, IUserService userService, IOrderService orderService)
        {
            _dbContext = dbContext;
            _fileService = fileService;
            _userService = userService;
            _orderService = orderService;
        }

        [HttpGet("checkout",Name ="client-order-checkout")]
        public async Task<IActionResult> CheckOut()
        {
            var model = new OrderProductsViewModel
            {
               Products = await _dbContext.BasketProducts.Where(bp=> bp.Basket.UserId == _userService.CurrentUser.Id)
               .Select(bp=> new OrderProductsViewModel.ItemViewModel 
               {
                  Name = bp.Book.Title,
                  Price = bp.Book.Price,
                  Quantity = bp.Quantity,
                  Total = bp.Book.Price * bp.Quantity,
               }).ToListAsync(),

               Summary = new OrderProductsViewModel.SummaryViewModel 
               {
                 Total = await _dbContext.BasketProducts.Where(bp=> bp.Basket.UserId == _userService.CurrentUser.Id)
                 .SumAsync(bp=> bp.Book.Price * bp.Quantity)
               }
            };
            return View(model);
        }

        [HttpPost("placeorder", Name = "client-order-placeorder")]
        public async Task<IActionResult> PlaceOrder()
        {
            var basketProducts = await _dbContext.BasketProducts
                        .Include(bp => bp.Book)
                        .Where(bp => bp.Basket!.UserId == _userService.CurrentUser.Id)
                        .ToListAsync();

            var order = await CreateOrder();

            await CreateFullFillOrderProductsAsync(order, basketProducts);

            order.SumTotalPrice = order.OrderProducts.Sum(o=> o.Total);

            await ResetBasketAsync(basketProducts);

            await _dbContext.SaveChangesAsync();

            return RedirectToRoute("client-account-orders");



            async Task ResetBasketAsync(List<BasketProduct> basketProducts)
            {
                await Task.Run(() => _dbContext.RemoveRange(basketProducts));
            }

            async Task CreateFullFillOrderProductsAsync(Order order ,List<BasketProduct> basketProducts)
            {
                foreach (var basketProduct in basketProducts)
                {
                    var orderProduct = new OrderProduct
                    {
                        OrderId = order.Id,
                        BookId = basketProduct.BookId,
                        Price = basketProduct.Book.Price,
                        Quantity = basketProduct.Quantity,
                        Total = basketProduct.Quantity * basketProduct.Book.Price

                    };
                 await _dbContext.AddAsync(orderProduct);
                }
            }

            async Task<Order> CreateOrder()
            {
                var order = new Order
                {
                    Id = await _orderService.GenerateUniqueTrackingCodeAsync(),
                    UserId = _userService.CurrentUser.Id,
                    Status = Database.Models.Enums.OrderStatus.Created,
                };

                await _dbContext.Orders.AddAsync(order);

                return order;

            }
           
        }


    }
}
