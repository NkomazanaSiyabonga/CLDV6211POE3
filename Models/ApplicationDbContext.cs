using Microsoft.EntityFrameworkCore;
using CloudDevelopmentPOE1.Models;



    namespace CloudDevelopmentPOE1.Models
    {
        public class ApplicationDbContext : DbContext
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
            {
            }

            public DbSet<Venue> Venue { get; set; }
            public DbSet<Event> Event { get; set; }
            public DbSet<Booking> Booking { get; set; }

        public DbSet<EventType> EventType { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
            {
                base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<EventType>().HasData(
new EventType { EventTypeID = 1, Name = "Motoring Race" },
new EventType { EventTypeID = 2, Name = "Football Match" },
new EventType { EventTypeID = 3, Name = "Product Launch" }
);

        }
    }
    }

    
