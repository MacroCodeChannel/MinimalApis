using Microsoft.EntityFrameworkCore;
using MinimalApi.Models;

namespace MinimalApi.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) 
            :base(options)
        { 

        }

        public DbSet<Student> Students { get; set; }
    }
}
