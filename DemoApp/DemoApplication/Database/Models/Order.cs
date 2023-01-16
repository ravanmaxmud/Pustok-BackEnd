﻿using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class Order : IAuditable
    {
        public string Id { get; set; }
        public string Status { get; set; }
        public decimal SumTotalPrice { get; set; }
        public DateTime CreatedAt { get; set ; }
        public DateTime UpdatedAt { get ; set ; }

        public int UserId { get; set; }
        public User User { get; set; }
    }
}
