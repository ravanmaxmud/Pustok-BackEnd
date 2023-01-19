using DemoApplication.Database.Models;

namespace DemoApplication.Services.Abstracts
{
    public interface IOrderService
    {
        Task<string> GenerateUniqueTrackingCodeAsync();
    }
}
