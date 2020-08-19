using CursoAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace CursoAPI.Data
{
    public class QuoteDbContext : DbContext
    {
        public QuoteDbContext(DbContextOptions<QuoteDbContext>options):base(options)
        {
            
        }
        public DbSet<Quote> Quotes { get; set; }

    }
}