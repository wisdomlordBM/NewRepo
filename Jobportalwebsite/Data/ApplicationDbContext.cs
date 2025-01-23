//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Jobportalwebsite.Models;
//using Jobportalwebsite.Services;

//namespace Jobportalwebsite.Data
//{
//    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
//    {
//        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

//        public DbSet<Job> Jobs { get; set; }
//        public DbSet<Application> Applications { get; set; }
//        public DbSet<Company> Companies { get; set; }
//        public DbSet<Notification> Notifications { get; set; }
//        public DbSet<Admin> Admins { get; set; }
//       public DbSet<Jobseekers> Jobseekers { get; set; }
//        public DbSet<Blog> Blogs { get; set; }
//        public DbSet<Comment> Comments { get; set; }
//        public DbSet<ApplicationUser> Users { get; set; }
//        protected override void OnModelCreating(ModelBuilder modelBuilder)
//        {
//            modelBuilder.Entity<Job>()
//                .Property(j => j.Salary)
//                .HasColumnType("decimal(18,2)");

//            base.OnModelCreating(modelBuilder);
//        }
//    }
//}

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Jobportalwebsite.Models;
using Jobportalwebsite.Services;

namespace Jobportalwebsite.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

        public DbSet<Job> Jobs { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Notification> Notifications { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Jobseekers> Jobseekers { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Reply> Replies { get; set; } // Add Replies DbSet
        public DbSet<ApplicationUser> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Job>()
                .Property(j => j.Salary)
                .HasColumnType("decimal(18,2)");

            // Configure the relationship between Comment and Reply
            modelBuilder.Entity<Reply>()
                .HasOne(r => r.Comment)
                .WithMany(c => c.Replies)
                .HasForeignKey(r => r.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}




