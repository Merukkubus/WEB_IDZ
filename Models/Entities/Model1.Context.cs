﻿//------------------------------------------------------------------------------
// <auto-generated>
//     Этот код создан по шаблону.
//
//     Изменения, вносимые в этот файл вручную, могут привести к непредвиденной работе приложения.
//     Изменения, вносимые в этот файл вручную, будут перезаписаны при повторном создании кода.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WebApplication1.Models.Entities
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class supermarketEntities : DbContext
    {
        public supermarketEntities()
            : base("name=supermarketEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<cash_reg> cash_reg { get; set; }
        public virtual DbSet<cashier> cashier { get; set; }
        public virtual DbSet<check> check { get; set; }
        public virtual DbSet<company> company { get; set; }
        public virtual DbSet<discount_card> discount_card { get; set; }
        public virtual DbSet<price> price { get; set; }
        public virtual DbSet<product> product { get; set; }
        public virtual DbSet<product_category> product_category { get; set; }
        public virtual DbSet<product_in_check> product_in_check { get; set; }
        public virtual DbSet<role> role { get; set; }
        public virtual DbSet<user> user { get; set; }
    }
}
