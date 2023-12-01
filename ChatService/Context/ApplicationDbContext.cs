using ChatService.Model.Catalog;
using Microsoft.EntityFrameworkCore;

namespace ChatService.Context
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<ChatMessage> ChatMessages { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatMessage>().ToTable("chatmessages", "catalog");
            base.OnModelCreating(modelBuilder);
        }
    }
}
