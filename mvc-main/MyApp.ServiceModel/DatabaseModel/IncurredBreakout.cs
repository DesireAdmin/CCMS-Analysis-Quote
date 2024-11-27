using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class IncurredBreakout
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }

        [Required]
        public string CostType { get; set; }

        public string? Description { get; set; }

        public decimal Rate { get; set; }

        public int Quantity { get; set; }

        public int Techs { get; set; }

        public decimal InitialCallSubTotal { get; set; }

        // Foreign key to link to QuoteModel
        public string QuoteModelId { get; set; }
    }
}
