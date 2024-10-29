using Microsoft.EntityFrameworkCore;

namespace CCMS_Analysis_Quote.Models
{
    public class ApplicationDbContext : DbContext  // Ensure ApplicationDbContext inherits from DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)  // Pass the options to the base DbContext constructor
        {
        }

        public DbSet<QuoteModel> QuoteModels { get; set; }  // Example DbSet
        public DbSet<IncurredBreakout> IncurredBreakouts { get; set; }
        public DbSet<ProposedBreakout> ProposedBreakouts { get; set; }
    }
}
