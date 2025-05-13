using System.Collections;
using CLDVpt1.Models;
using Microsoft.EntityFrameworkCore;


namespace CLDVpt1.Models
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Venues> Venues { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public DbSet<Events> Events { get; set; }
        // (and others like Customers, Venues if needed)


    }
}
