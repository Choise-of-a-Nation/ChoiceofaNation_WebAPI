using Logic.Entity;
using Microsoft.AspNetCore.Identity;
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

            //optionsBuilder.UseNpgsql(@"Server=localhost:5433;Database=cn_db_1;User ID=postgres;Password=Vladadmin2222");

            optionsBuilder.UseSqlServer("Data Source=SQL6033.site4now.net;Initial Catalog=db_ab26d5_cn1;User Id=db_ab26d5_cn1_admin;Password=Vlad2005;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction);  // ✅ Виправлення

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction);  // ✅ Виправлення

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

            // Обмежуємо довжину ключів до 191 символу
            modelBuilder.Entity<IdentityUser>(b =>
            {
                b.Property(u => u.Id).HasColumnType("VARCHAR(191)");
            });

            modelBuilder.Entity<IdentityRole>(b =>
            {
                b.Property(r => r.Id).HasColumnType("VARCHAR(191)");
            });

            modelBuilder.Entity<IdentityUserRole<string>>(b =>
            {
                b.Property(ur => ur.UserId).HasColumnType("VARCHAR(191)");
                b.Property(ur => ur.RoleId).HasColumnType("VARCHAR(191)");
            });

            modelBuilder.SeedRoles();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }

    }
}

