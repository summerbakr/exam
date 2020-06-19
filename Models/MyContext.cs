using Microsoft.EntityFrameworkCore;

namespace Exam.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users{get;set;}

        public DbSet<Event> Events{get;set;}

        public DbSet<Participant> Participants{get;set;}

    
    }
}