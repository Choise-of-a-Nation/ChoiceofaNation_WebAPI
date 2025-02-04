using Logic.Entity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Data
{
    public static class MockData
    {
        public static void SeedRoles(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Roles>().HasData(new Roles[]
            {
                new Roles() { Id = TypesD.Full.ToString(), Name = "Адмін" },
                new Roles() { Id = TypesD.Client.ToString(), Name = "Клієнт" },
            });
        }
    }
}
