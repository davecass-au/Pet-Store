using Microsoft.EntityFrameworkCore;

namespace PetStore.Services
{
    internal class PetStoreContext : DbContext
    {
        public PetStoreContext(DbContextOptions<PetStoreContext> options) : base(options)
        {            
        }

        public DbSet<Pet>? Pets { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pet>()            
                .ToTable("Pets");
        }
    }
}
