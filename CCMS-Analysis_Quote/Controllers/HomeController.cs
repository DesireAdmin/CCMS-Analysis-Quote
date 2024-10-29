using Azure.Storage.Blobs;
using CCMS_Analysis_Quote.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Diagnostics;

namespace CCMS_Analysis_Quote.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        private readonly string _azureConnectionString = "AccountName=devstoreaccount1;AccountKey=Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==;DefaultEndpointsProtocol=http;BlobEndpoint=http://127.0.0.1:10000/devstoreaccount1;QueueEndpoint=http://127.0.0.1:10001/devstoreaccount1;TableEndpoint=http://127.0.0.1:10002/devstoreaccount1;"; // Set your Azure connection string
        private readonly string _containerName = "signatures";

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SubmitForm(QuoteModel model)
        {
            // Handle the signature upload
            if (model.SignatureFile != null && model.SignatureFile.Length > 0)
            {
                string signatureUrl = await UploadSignatureAsync(model.SignatureFile, model.Id);
                if (string.IsNullOrEmpty(signatureUrl))
                {
                    ModelState.AddModelError("SignatureFile", "Failed to upload signature file.");
                    return View(model);
                }
                model.SignatureUrl = signatureUrl; 
            }
            else
            {
                model.SignatureUrl = ""; 
            }

            
            if (model.ProposedBreakouts != null && model.ProposedBreakouts.Count > 0)
            {
                foreach (var proposed in model.ProposedBreakouts)
                {
                    proposed.Id = GenerateUniqueId("PROPOSED_"); 
                    proposed.QuoteModelId = model.Id; 
                    _context.ProposedBreakouts.Add(proposed);
                }
            }

            
            if (model.IsIncurredCost && model.IncurredBreakouts != null && model.IncurredBreakouts.Count > 0)
            {
                foreach (var incurred in model.IncurredBreakouts)
                {
                    incurred.Id = GenerateUniqueId("INCURRED_"); 
                    incurred.QuoteModelId = model.Id; 
                    _context.IncurredBreakouts.Add(incurred);
                }
            }

            _context.QuoteModels.Add(model);
            await _context.SaveChangesAsync(); 

            return RedirectToAction("Index");
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

        //[HttpPost]
        //public IActionResult SubmitForm(QuoteModel model)
        //{
        //    //_context.SaveChanges();

        //        if (model.ProposedBreakouts != null && model.ProposedBreakouts.Count > 0)
        //        {
        //            foreach (var proposed in model.ProposedBreakouts)
        //            {
        //                proposed.Id = GenerateUniqueId("PROPOSED_"); 
        //                proposed.QuoteModelId = model.Id; 
        //                _context.ProposedBreakouts.Add(proposed);
        //            }
        //        }

        //        if (model.IsIncurredCost && model.IncurredBreakouts != null && model.IncurredBreakouts.Count > 0)
        //        {
        //            foreach (var incurred in model.IncurredBreakouts)
        //            {
        //                incurred.Id = GenerateUniqueId("INCURRED_"); 
        //                incurred.QuoteModelId = model.Id; 
        //                _context.IncurredBreakouts.Add(incurred);
        //            }
        //        }
        //        _context.QuoteModels.Add(model);

        //        _context.SaveChanges(); // Save all changes

        //        return RedirectToAction("Index");           
        //}

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult NewForm()
        {
            return View();
        }
        public string GenerateUniqueId(string prefix)
        {
            return prefix + Guid.NewGuid().ToString();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
