using Logic.Entity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public class DbContext : IdentityDbContext
    {
        public DbContext()
        {

        }

        public DbContext(DbContextOptions options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseNpgsql(@"Server=localhost:5433;Database=cn_db;User ID=postgres;Password=Vladadmin2222");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.SeedRoles();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }

    }
}

