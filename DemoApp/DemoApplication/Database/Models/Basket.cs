﻿using DemoApplication.Database.Models.Common;

namespace DemoApplication.Database.Models
{
    public class Basket : BaseEntity, IAuditable
    {
        public int UserId { get; set; }
        public User? User { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public List<BasketProduct>? BasketProducts { get; set; }
    }
}
