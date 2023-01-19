using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using Microsoft.EntityFrameworkCore;
using System;

namespace DemoApplication.Services.Concretes
{
    public class OrderService : IOrderService
    {

        private readonly DataContext _context;
        private const string ORDER_TRACKING_CODE = "OR";
        private const int ORDER_TRACKINH_MIN_RANGE = 10_000;
        private const int ORDER_TRACKINH_MAX_RANGE = 100_000;

        public OrderService(DataContext context)
        {
            _context = context;
        }

        public async Task<string> GenerateUniqueTrackingCodeAsync()
        {
            string token = string.Empty;
            do
            {
                token = GenerateRandomTrackingCode();
            } while (await _context.Orders.AnyAsync(O=> O.Id == token));

            return token;
        }
        private string GenerateRandomTrackingCode() 
        {
            return $"{ORDER_TRACKING_CODE}{Random.Shared.Next(ORDER_TRACKINH_MIN_RANGE,ORDER_TRACKINH_MAX_RANGE)}";        
        }
    }
}