using LearnOnline.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace LearnOnline.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<Auth> auths { get; set; }
        public DbSet<Upload> Uploads { get; set; }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Cart> Carts { get; set; } // Add this line

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define the relationship between Cart and Course
            modelBuilder.Entity<Cart>()
                .HasOne(c => c.Course)
                .WithMany()
                .HasForeignKey(c => c.CourseId);

            base.OnModelCreating(modelBuilder);
        }
    }
}
