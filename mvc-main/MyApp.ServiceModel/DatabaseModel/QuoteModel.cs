using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using ServiceStack.DataAnnotations;
using RequiredAttribute = System.ComponentModel.DataAnnotations.RequiredAttribute;

namespace MyApp.ServiceModel.DatabaseModel
{
    public class QuoteModel
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [Display(Name = "SMG Vendor PO")]
        [Required(ErrorMessage = "SMG Vendor PO is required.")]
        [RegularExpression(@"^\d{6}(-\d{2})?$", ErrorMessage = "SMG Vendor PO must be in this digit format ###### or ######-##.")]
        public string SMG_Vendor_PO { get; set; }

        public DateTime Date { get; set; }

        [Display(Name = "Store Name")]
        [Required(ErrorMessage = "Client name is required.")]
        [System.ComponentModel.DataAnnotations.StringLength(40, ErrorMessage = "Client name cannot exceed 40 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Client name can only contain alphabetic characters and spaces.")]
        public string SMG_CLIENT { get; set; }

        [Display(Name = "Store number")]
        [RegularExpression(@"^\d+$", ErrorMessage = "Store number must be numeric.")]
        public string? StoreNumber { get; set; }

        public string? Location { get; set; }

        [Required(ErrorMessage = "Vendor is required.")]
        public string Vendor { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email format.")]
        public string Email { get; set; }

        [Display(Name = "Service Representative Name")]
        [System.ComponentModel.DataAnnotations.StringLength(40, ErrorMessage = "Service Representative Name cannot exceed 40 characters.")]

        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Service Representative Name can only contain letters and spaces.")]
        public string ServiceRepName { get; set; }

        public bool IsIncurredCost { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Service Date is required.")]
        public DateTime ServiceDate { get; set; }

        [System.ComponentModel.DataAnnotations.StringLength(250, ErrorMessage = "Incurred Issue Description cannot exceed 250 characters.")]
        public string? IncurredIssueDescription { get; set; } = "dakj";

        public decimal QuoteGrandTotal { get; set; }
        [System.ComponentModel.DataAnnotations.StringLength(250, ErrorMessage = "Proposed Issue Description cannot exceed 250 characters.")]

        public string? ProposedIssueDescription { get; set; }
        public List<ProposedBreakout> ProposedBreakouts { get; set; } = new List<ProposedBreakout>();
        public List<IncurredBreakout> IncurredBreakouts { get; set; } = new List<IncurredBreakout>();

        public IncurredTotal IncurredTotals { get; set; } = new IncurredTotal();
        public ProposedTotal ProposedTotals { get; set; } = new ProposedTotal();
        public string AttachmentUrl { get; set; }
        [NotMapped]
        [Required(ErrorMessage = "AttachmentFile is required.")]
        public IFormFile AttachmentFile { get; set; }

        public string? Disclaimer { get; set; }

        //[System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Signature Name of Services is required.")]
        [Display(Name = "SignatureName")]
        [Required(ErrorMessage = "Signature Name is required.")]
        [System.ComponentModel.DataAnnotations.StringLength(40, ErrorMessage = "Signature Name cannot exceed 40 characters.")]
        [RegularExpression(@"^[a-zA-Z\s]+$", ErrorMessage = "Signature Name can only contain letters and spaces.")]
        public string SignatureName { get; set; }

        [System.ComponentModel.DataAnnotations.Required(ErrorMessage = "Checked of Services is required.")]
        public bool IsChecked { get; set; } = false;

        [Display(Name = "Time Option")]
        [Required(ErrorMessage = "Time Option is required.")]
        public string TimeOption { get; set; } // "Number" or "Range"

        [Display(Name = "Time Number")]
        [System.ComponentModel.DataAnnotations.Range(1, int.MaxValue, ErrorMessage = "Time Number must be greater than 0.")]
        public int? TimeNumber { get; set; } // For "Number" option

        [Display(Name = "Time Unit")]
        public string TimeUnit { get; set; } = "0";// "Day" or "Week" for "Number" option

        [Display(Name = "Time Range From")]
        public int? TimeRangeFrom { get; set; } // For "Range" option

        [Display(Name = "Time Range To")]
        public int? TimeRangeTo { get; set; }

    }
}
