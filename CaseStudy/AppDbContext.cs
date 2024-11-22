
using Microsoft.EntityFrameworkCore;
using CaseStudy.Models;

namespace CaseStudy
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }

        public DbSet<Fund> Funds { get; set; }
        public DbSet<FundReturn> FundReturns { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Fund>(entity =>
            {
                entity.ToTable("fund");

                entity.HasKey(e => e.Id);

                entity.Property(e => e.Code)
                    .HasColumnType("varchar")
                    .HasMaxLength(100)
                    .IsRequired();

                entity.Property(e => e.Name)
                    .HasColumnType("varchar")
                    .HasMaxLength(500) 
                    .IsRequired();

                entity.Property(e => e.Type)
                    .HasColumnType("varchar")
                    .HasMaxLength(200)
                    .IsRequired();
            });

           

            modelBuilder.Entity<FundReturn>(entity =>
            {
                entity.ToTable("fund_returns");
                entity.HasKey(e => e.Id);

                entity.Property(e => e.OneMonthReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false); // Nullable 

                entity.Property(e => e.ThreeMonthReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false); // Nullable 

                entity.Property(e => e.SixMonthReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false); // Nullable 

                entity.Property(e => e.YearToDateReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false); // Nullable 

                entity.Property(e => e.OneYearReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false); // Nullable

                entity.Property(e => e.ThreeYearReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false)
                      .HasColumnOrder(7); // Nullable

                entity.Property(e => e.FiveYearReturn)
                      .HasPrecision(18, 4)
                      .IsRequired(false)
                      .HasColumnOrder(8); // Nullable

                entity.HasOne(e => e.Fund)
                      .WithMany(f => f.Returns)
                      .HasForeignKey(e => e.FundId)
                      .OnDelete(DeleteBehavior.Cascade);
            });

        }
    }
}