using CSVEndpoint.Models;
using Microsoft.EntityFrameworkCore;

namespace CSVEndpoint.Data
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public AppDbContext() { }
        public DbSet<CVSMetadataModel> CSVMetadataModel { get; set; }  

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            modelBuilder.Entity<CVSMetadataModel>()
                .HasKey(m => m.Id);
        }
    }
}
