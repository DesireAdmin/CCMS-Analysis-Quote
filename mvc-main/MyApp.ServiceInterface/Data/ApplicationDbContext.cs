using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyApp.ServiceModel.DatabaseModel;
namespace MyApp.Data;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) 
    : IdentityDbContext<ApplicationUser>(options)
{
    public DbSet<QuoteModel> QuoteModels { get; set; }  // Example DbSet
    public DbSet<IncurredBreakout> IncurredBreakouts { get; set; }
    public DbSet<ProposedBreakout> ProposedBreakouts { get; set; }
    public DbSet<IncurredTotal> IncurredTotals { get; set; }
    public DbSet<ProposedTotal> ProposedTotals { get; set; }
    public DbSet<HistoryLogs> HistoryLogs { get; set; }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // QuoteModel - ProposedBreakout relationship
        modelBuilder.Entity<ProposedBreakout>()
            .HasOne<QuoteModel>()
            .WithMany(q => q.ProposedBreakouts)
            .HasForeignKey(p => p.QuoteModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // QuoteModel - IncurredBreakout relationship
        modelBuilder.Entity<IncurredBreakout>()
            .HasOne<QuoteModel>()
            .WithMany(q => q.IncurredBreakouts)
            .HasForeignKey(i => i.QuoteModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // ProposedBreakout - ProposedTotal relationship
        modelBuilder.Entity<ProposedTotal>()
            .HasOne<QuoteModel>()
            .WithOne(p => p.ProposedTotals)
            .HasForeignKey<ProposedTotal>(p => p.QuoteModelId)
            .OnDelete(DeleteBehavior.Cascade);

        // IncurredBreakout - IncurredTotal relationship
        modelBuilder.Entity<IncurredTotal>()
            .HasOne<QuoteModel>()
            .WithOne(i => i.IncurredTotals)
            .HasForeignKey<IncurredTotal>(i => i.QuoteModelId)
            .OnDelete(DeleteBehavior.Cascade);

        base.OnModelCreating(modelBuilder);
    }
}