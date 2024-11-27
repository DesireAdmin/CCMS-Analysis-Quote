using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class ProposedTotal
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; }
        public decimal ProposedSubTotal { get; set; }
        public decimal ProposedTax { get; set; }
        public decimal ProposedInitialCallTotal { get; set; }
        public string QuoteModelId { get; set; }
    }
}
