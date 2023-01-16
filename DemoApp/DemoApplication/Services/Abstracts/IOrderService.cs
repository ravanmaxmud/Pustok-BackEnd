using DemoApplication.Areas.Client.ViewModels.Order;
using DemoApplication.Database.Models;

namespace DemoApplication.Services.Abstracts
{
    public interface IOrderService
    {
        Task<Order> AddOrderAsync(OrderViewModel model, decimal SumTotal);
        Task AddOrderProductAsync(OrderViewModel model);
    }
}
