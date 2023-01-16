﻿using DemoApplication.Database.Configurations;
using DemoApplication.Database.Models;
using DemoApplication.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace DemoApplication.Database
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Book> Books { get; set; }
        public DbSet<BookImage> BookImages { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Contact> Contacts { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<BookCategory> BookCategories { get; set; }
        public DbSet<Color> Colors { get; set; }
        public DbSet<Subscribes> Subscribes { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<BasketProduct> BasketProducts { get; set; }
        public DbSet<Addres> Address { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Slider> Sliders { get; set; }
        public DbSet<UserActivation> UserActivations { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderProduct> OrderProducts { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly<Program>();
        }
    }
}
