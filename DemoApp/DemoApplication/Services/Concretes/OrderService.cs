using DemoApplication.Database;
using DemoApplication.Database.Models;
using DemoApplication.Services.Abstracts;
using System;

namespace DemoApplication.Services.Concretes
{
    public class OrderService : IOrderService
    {

        private readonly DataContext _context;

        public OrderService(DataContext context)
        {
            _context = context;
        }

        static Random _random = new Random();
        public string OrderCode
        {
            get
            {
                bool go = true;
                string newCode = "OR" + _random.Next(1000, 10000);


                while (go)
                {
                    int lastId = 0;


                    foreach (Order order in _context.Orders)
                    {
                        if (order.Id == newCode)
                        {
                            newCode = "OR" + _random.Next(1000, 10000);

                        }
                        lastId++;
                    }

                    if (lastId == _context.Orders.Count())
                    {
                        go = false;

                    }

                }


                return newCode;
            }


        }
    }
}