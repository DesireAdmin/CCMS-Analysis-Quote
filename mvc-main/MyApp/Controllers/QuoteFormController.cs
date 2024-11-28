using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Protocols;
using MyApp.Data;
using MyApp.ServiceModel.DatabaseModel;
using System.Net.Mail;

namespace MyApp.Controllers
{
    public class QuoteFormController : Controller
    {
        private readonly ILogger<QuoteFormController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly string _azureConnectionString = string.Empty;
        private readonly string _containerName = string.Empty;
        private readonly IConfiguration _configuration;
        private readonly IBradEmailSender _bradEmail;
        public QuoteFormController(ILogger<QuoteFormController> logger, ApplicationDbContext context, IConfiguration configuration, IBradEmailSender bradEmail)
        {
            _logger = logger;
            _context = context;
            _configuration = configuration;
            _bradEmail = bradEmail;
            _azureConnectionString = _configuration.GetValue<string>("AzureExplorer:PrimaryConnectionString")!;
            _containerName = _configuration.GetValue<string>("AzureExplorer:ContainerName")!;
        }
        [Authorize]
        public IActionResult Index()
        {
            var data = _context.QuoteModels.ToList();

            return View(data);
        }


        public IActionResult CreateQuoteForm()
        {


            return View();
        }

        [HttpGet]
        public ActionResult GetQuoteDetails(string id)
        {
            // Retrieve the quote based on the provided ID, including related breakout data
            var quote = _context.QuoteModels
                .Include(q => q.IncurredBreakouts)
                .Include(q => q.ProposedBreakouts)
                .Include(q => q.IncurredTotals)
                .Include(q => q.ProposedTotals)
                .FirstOrDefault(q => q.Id == id);

            if (quote == null)
            {
                return Json(new { success = false, message = "Quote not found" });
            }

            // Filter the related breakout data based on the QuoteModelId, if necessary
            quote.ProposedBreakouts = quote.ProposedBreakouts
                .Where(f => f.QuoteModelId == id).ToList();

            quote.IncurredBreakouts = quote.IncurredBreakouts
                .Where(f => f.QuoteModelId == id).ToList();

            // Return the quote data along with the filtered breakouts
            return Json(new { success = true, data = quote });
        }

        public IActionResult SubmitSuccess()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateQuoteForm(QuoteModel model)
        {
            if (model.IsChecked == false)
            {
                ModelState.AddModelError("IsChecked", "Please confirm that the provided information is accurate by checking the confirmation box before submitting the form.");
                return View("CreateQuoteForm", model);
            }
            if (string.IsNullOrEmpty(model.SignatureName))
            {
                ModelState.AddModelError("SignatureName", "This field is required.");
                return View("CreateQuoteForm", model);
            }
            // Handle the signature upload
            if (model.AttachmentFile != null && model.AttachmentFile.Length > 0)
            {
                string signatureUrl = await UploadSignatureAsync(model.AttachmentFile, model.Id);
                if (string.IsNullOrEmpty(signatureUrl))
                {
                    ModelState.AddModelError("AttachmentFile", "Failed to upload signature file.");
                    return View(model);
                }
                else
                {
                    long fileSizeInBytes = model.AttachmentFile.Length; // Size in bytes
                    long fileSizeInKilobytes = fileSizeInBytes / 1024; // Size in KB
                    long fileSizeInMegabytes = fileSizeInKilobytes / 1024; // Size in MB
                    if (fileSizeInMegabytes > 10)
                    {
                        ModelState.AddModelError("AttachmentFile", "Please upload file less than 10MB.");
                        return View("CreateQuoteForm", model);
                    }
                }
                model.AttachmentUrl = signatureUrl;
            }
            else
            {
                model.AttachmentUrl = "";
            }

            // Store ProposedBreakouts data
            if (model.ProposedBreakouts != null)
            {
                foreach (var proposed in model.ProposedBreakouts)
                {
                    proposed.Id = proposed.Id ?? GenerateUniqueId("PROPOSED_");
                    proposed.QuoteModelId = model.Id;
                    _context.ProposedBreakouts.Add(proposed);
                }
            }

            // Store IncurredBreakouts and IncurredTotals based on IsIncurredCost
            if (!model.IsIncurredCost)
            {
                model.IncurredBreakouts = null;
                //model.IncurredTotals = null;
            }
            else
            {
                if (model.IncurredBreakouts != null)
                {
                    foreach (var incurred in model.IncurredBreakouts)
                    {
                        if (!string.IsNullOrEmpty(incurred.Description))
                        {
                            incurred.Id = incurred.Id ?? GenerateUniqueId("INCURRED_");
                            incurred.QuoteModelId = model.Id;
                            _context.IncurredBreakouts.Add(incurred);
                        }
                    }
                }
            }
            var grandTotal = model.QuoteGrandTotal;
            _context.QuoteModels.Add(model);
            await _context.SaveChangesAsync();

            var pdfdata = await new MyApp.ServiceModel.Helper.PdfGenerator(_configuration).GeneratePdf(model);

            var emailbody = _bradEmail.returnHtmlBody();

            emailbody = emailbody.Replace("{{Clientname}}", model.SMG_CLIENT);
            emailbody = emailbody.Replace("{{SMGVendorPO}}", model.SMG_Vendor_PO);
            emailbody = emailbody.Replace("{{SMGClient}}", model.SMG_CLIENT);
            emailbody = emailbody.Replace("{{StoreNumber}}", model.StoreNumber);
            emailbody = emailbody.Replace("{{Email}}", model.Email);
            emailbody = emailbody.Replace("{{Date}}", model.Date.ToString());
            emailbody = emailbody.Replace("{{Location}}", model.Location);
            await _bradEmail.SendEmailAsync(model.Email, "Brad email", emailbody, pdfdata, model.AttachmentFile);

            return RedirectToAction("SubmitSuccess");
        }

        private async Task<string> UploadSignatureAsync(IFormFile file, string quoteModelId)
        {
            // Create a blob service client
            BlobServiceClient blobServiceClient = new BlobServiceClient(_azureConnectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(_containerName);
            await containerClient.CreateIfNotExistsAsync(); // Create the container if it doesn't exist

            // Generate a unique blob name
            var blobName = $"{quoteModelId}/{file.FileName}";
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            using (var stream = file.OpenReadStream())
            {
                await blobClient.UploadAsync(stream, true);
            }

            return blobClient.Uri.ToString();
        }

        public string GenerateUniqueId(string prefix)
        {
            return prefix + Guid.NewGuid().ToString();
        }
    }
}
