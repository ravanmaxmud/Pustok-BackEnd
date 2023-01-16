using DemoApplication.Areas.Client.ViewModels.Order;
using DemoApplication.Database.Models;

namespace DemoApplication.Services.Abstracts
{
    public interface IOrderService
    {
        public string OrderCode { get; }
    }
}
