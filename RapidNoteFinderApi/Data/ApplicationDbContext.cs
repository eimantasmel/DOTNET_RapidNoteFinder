using Microsoft.EntityFrameworkCore;
using RapidNoteFinderApi.Models;

namespace RapidNoteFinderApi.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Note> Notes { get; set; }
    }
}
