using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCMS_Analysis_Quote.Models
{
    public class ProposedBreakout
    {
        [Key]
        public string Id { get; set; }

        [Required]
        public string CostType { get; set; }

        public string Description { get; set; }

        public decimal Rate { get; set; }

        public int Quantity { get; set; }

        public int Techs { get; set; }

        public decimal InitialCallSubTotal { get; set; }

        // Foreign key to link to QuoteModel
        public string QuoteModelId { get; set; }
    }

    public class IncurredBreakout
    {
        [Key]
        public string Id { get; set; } 

        [Required]
        public string CostType { get; set; }

        public string Description { get; set; }

        public decimal Rate { get; set; }

        public int Quantity { get; set; }

        public int Techs { get; set; }

        public decimal InitialCallSubTotal { get; set; }

        // Foreign key to link to QuoteModel
        public string QuoteModelId { get; set; }
    }
}
