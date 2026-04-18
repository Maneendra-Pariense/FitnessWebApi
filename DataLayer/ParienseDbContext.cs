using FitnessApi.DataLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace FitnessApi.DataLayer
{
    public class ParienseDbContext : DbContext
    {
        public ParienseDbContext(DbContextOptions options) : base(options)
        {

        }
        public DbSet<SugarLog> SugarLogs { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SugarLog>(entity =>
            {
                // Ensure only one log per user per calendar date. We store Date as date-only in the DB.
                entity.Property(e => e.Date).HasColumnType("date");
                entity.HasIndex(e => new { e.UserId, e.Date }).IsUnique();

                entity.OwnsOne(e => e.Fasting);
                entity.OwnsOne(e => e.AfterBreakfast);
                entity.OwnsOne(e => e.BeforeLunch);
                entity.OwnsOne(e => e.AfterLunch2hrs);
                entity.OwnsOne(e => e.BeforeDinner);
                entity.OwnsOne(e => e.AfterDinner2hrs);
                entity.OwnsOne(e => e.Between2am3am);
            });
        }

    }
}
