using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CCMS_Analysis_Quote.Models
{
    public class QuoteModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Required(ErrorMessage = "SMG Vendor PO is required.")]
        [RegularExpression(@"^\d{6}(-\d{2})?$", ErrorMessage = "SMG Vendor PO must be in the format ###### or ######-##.")]
        public string SMG_Vendor_PO { get; set; }

        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Client name is required.")]
        public string SMG_CLIENT { get; set; }
        public string StoreNumber { get; set; }

        [Required(ErrorMessage = "Location is required.")]
        public string Location { get; set; }

        [Required(ErrorMessage = "City and State are required.")]
        public string CityState { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        public string Vendor { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }
        public string ServiceRepName { get; set; }

        public bool IsIncurredCost { get; set; }

        public List<ProposedBreakout> ProposedBreakouts { get; set; } = new List<ProposedBreakout>();
        public List<IncurredBreakout> IncurredBreakouts { get; set; } = new List<IncurredBreakout>();
        public string SignatureUrl { get; set; }
        [NotMapped]
        public IFormFile SignatureFile { get; set; }
    }
}
