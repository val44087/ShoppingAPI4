﻿using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShoppingApi.Domain
{
    public class ShoppingDataContext : DbContext
    {

        public ShoppingDataContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<ShoppingItem> ShoppingItems { get; set; }

        public DbSet<CurbsideOrder> CurbsideOrders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ShoppingItem>().Property(p => p.Description)
                 .HasMaxLength(300);
        }
    }
}
