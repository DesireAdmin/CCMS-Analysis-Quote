using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class IncurredTotal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public decimal IncurredSubTotal { get; set; }
        public decimal IncurredTax { get; set; }
        public decimal IncurredInitialCallTotal { get; set; }
        public string QuoteModelId { get; set; }
    }
}
