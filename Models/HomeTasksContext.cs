using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace HomeTaskApi.Models
{
    public class HomeTasksContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<HomeTask> HomeTasks { get; set; }
        public HomeTasksContext(DbContextOptions<HomeTasksContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
