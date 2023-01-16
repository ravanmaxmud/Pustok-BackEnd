using DemoApplication.Areas.Client.ViewModels.Order;
using DemoApplication.Contracts.Order;
using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;

namespace DemoApplication.Services.Concretes
{
    public class OrderService : IOrderService
    {
        private readonly DataContext _dataContext;
        private readonly IUserService _userService;

        private const int MIN_RANDOM_NUMBER = 10000;
        private const int MAX_RANDOM_NUMBER = 100000;
        private const string PREFIX = "OR";

        public OrderService(DataContext dataContext, IUserService userService)
        {
            _dataContext = dataContext;
            _userService = userService;
        }

        public async Task<Order> AddOrderAsync(OrderViewModel model, decimal SumTotal)
        {
            ArgumentNullException.ThrowIfNull(SumTotal);

            var order = new Order
            {
                UserId = _userService.CurrentUser.Id,
                OrderCode = GenerateOrderCode(),
                SumTotalPrice = SumTotal,
                User = _userService.CurrentUser,
                Status = Status.Created,
                CreatedAt = DateTime.Now,

            };

            await _dataContext.Orders.AddAsync(order);

            return order;
        }

        public async Task AddOrderProductAsync(OrderViewModel model)
        {
            ArgumentNullException.ThrowIfNull(model);

            foreach (var orderProductModel in model.ProductList)
            {
                var orderProduct = new OrderProduct
                {
                    BookId = orderProductModel.Id,
                    Quantity = orderProductModel.Quantity,
                };
                await _dataContext.OrderProducts.AddAsync(orderProduct);
            }
        }

        private string GenerateNumber()
        {
            Random random = new Random();
            return random.Next(MIN_RANDOM_NUMBER, MAX_RANDOM_NUMBER).ToString();
        }

        private string GenerateOrderCode()
        {
            var orderCode = string.Empty;
            while (true)
            {
                orderCode = $"{PREFIX}{GenerateNumber()}";

                if (!_dataContext.Orders.Any(o => o.OrderCode == orderCode))
                {
                    return orderCode;
                }
            }
        }
    }
}
