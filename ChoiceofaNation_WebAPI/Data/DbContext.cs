using ChoiceofaNation_WebAPI.Logic.Entity;
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

            optionsBuilder.UseSqlServer("Server=tcp:cn-server.database.windows.net,1433;Initial Catalog=cn-db;Persist Security Info=False;User ID=Windbreaker;Password=Vladadmin2222;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityUser>()
                .WithMany()
                .HasForeignKey(ur => ur.UserId)
                .OnDelete(DeleteBehavior.NoAction);  

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasOne<IdentityRole>()
                .WithMany()
                .HasForeignKey(ur => ur.RoleId)
                .OnDelete(DeleteBehavior.NoAction);  

            modelBuilder.Entity<IdentityUserRole<string>>()
                .HasKey(r => new { r.UserId, r.RoleId });

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

            modelBuilder.Entity<Comment>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.Entity<Topic>()
                .Property(c => c.Id)
                .ValueGeneratedOnAdd();

            modelBuilder.SeedRoles();
        }

        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<News> News { get; set; }
        public virtual DbSet<Roles> Roles { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<Topic> Topics { get; set; }
        public virtual DbSet<HistoryWiki> HistoryWikis { get; set; }

    }
}

