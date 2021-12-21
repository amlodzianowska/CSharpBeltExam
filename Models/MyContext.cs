using Microsoft.EntityFrameworkCore;

namespace beltExam.Models
{ 
    public class MyContext : DbContext 
    { 
        public MyContext(DbContextOptions options) : base(options) { }
        // the "Dishes" table name will come from the DbSet variable name
        public DbSet<User> Users { get; set; }
        public DbSet<DojoActivity> DojoActivities { get; set; }
        public DbSet<Attendance> Attendance { get; set; }
        
    }
}