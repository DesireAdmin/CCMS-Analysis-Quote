using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using PuppeteerSharp;
using PuppeteerSharp.Media;
using Azure.Storage.Blobs;
using System;
using System.IO;
using System.Threading.Tasks;
using MyApp.ServiceModel.DatabaseModel;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Azure;

namespace MyApp.ServiceModel.Helper
{

    public class JsonToPdfGenerator
    {
        private readonly IConfiguration _configuration;
        private string connectionString { get; set; }
        private string containerName { get; set; }
        private string blobName = "output.pdf"; // Name for the PDF in Blob Storage

        public JsonToPdfGenerator(IConfiguration _configuration)
        {
            connectionString = _configuration.GetSection("AzureExplorer:PrimaryConnectionString").Value;
            containerName = _configuration.GetSection("AzureExplorer:ContainerName2").Value;

        }
        public async Task<byte[]> GeneratePdfAsync(QuoteModel ouoteModel)
        {
            // Ensure Chromium is downloaded
            await new BrowserFetcher().DownloadAsync();

            // Load HTML template
            string htmlTemplate = PopulateData(ouoteModel);

            // Generate PDF as a byte array (without saving to local disk)
            byte[] pdfData;
            using (var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true }))
            using (var page = await browser.NewPageAsync())
            {
                // Load HTML content
                await page.SetContentAsync(htmlTemplate);

                // PDF options
                var pdfOptions = new PdfOptions
                {
                    Format = PaperFormat.A4,
                    PrintBackground = true,
                    MarginOptions = new MarginOptions
                    {
                        Top = "0px",
                        Bottom = "0px",
                        Left = "0px",
                        Right = "0px"
                    }
                };

                // Generate PDF as byte array
                pdfData = await page.PdfDataAsync(pdfOptions);
                await UploadPdfToAzureBlobAsync(pdfData, ouoteModel.Id);
                Console.WriteLine("PDF generated successfully.");
                return pdfData;
            }

            return null;
            // Upload PDF to Azure Blob Storage
            //  await UploadPdfToAzureBlobAsync(pdfData);
        }

        private async Task UploadPdfToAzureBlobAsync(byte[] pdfData, string id)
        {
            // Create a BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get the container reference
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            // Get the blob reference
            BlobClient blobClient = containerClient.GetBlobClient(id + "_" + blobName);

            // Upload the PDF data as a blob
            using (var stream = new MemoryStream(pdfData))
            {
                await blobClient.UploadAsync(stream, overwrite: true);
                Console.WriteLine("PDF uploaded to Azure Blob Storage.");
            }
        }

        private string PopulateData(QuoteModel quoteModel)
        {
            // HTML templates for the tables
            var incurredTableTemplate = @"<div class=""table-responsive"">
        <table class=""table table-bordered proposed-quote-table"">
            <thead class=""thead-light"">
                <tr>
                    <th>Cost Type</th>
                    <th>Description</th>
                    <th>Rate</th>
                    <th>Quantity</th>
                    <th>#Techs</th>
                    <th>Initial Call Sub Total</th>
                </tr>
            </thead>
            <tbody>
                {{incurredBreakoutsRows}}
            </tbody>
            <tfoot>
                <tr>
                    <td>Sub Total</td>
                    <td colspan=""5"">
                        <input type=""text"" class=""form-control"" value=""{{IncurredSubTotal}}"" readonly>
                    </td>
                </tr>
                <tr>
                    <td>Tax</td>
                    <td colspan=""5"">
                        <input type=""text"" class=""form-control"" value=""{{IncurredTax}}"" readonly>
                    </td>
                </tr>
                <tr>
                    <td>Initial Call Total</td>
                    <td colspan=""5"">
                        <input type=""text"" class=""form-control"" value=""{{IncurredInitialCallTotal}}"" readonly>
                    </td>
                </tr>
            </tfoot>
        </table>
    </div>";

            var serviceTableTemplate = @"<div class=""container-fluid mt-5"">
        <div class=""initial-call-breakout"">
            <div class=""d-flex justify-content-between align-items-center mb-3"">
                <h4 style=""text-align: left; margin-bottom: 0;"">INITIAL CALL BREAKOUT</h4>
                <div>
                    <span style=""font-weight: bold; color: #333;"">Service Date:</span>
                    <span style=""font-size: 14px; color: #555;"">{{serviceDate}}</span>
                </div>
            </div>
            {{incurredTable}}
        </div>
    </div>";

            var partsLeadNumberTemplate = @"<div class=""DocAttachAndPartLeadTime"" style=""{{PartsLead-Style}} text-align: center ;"" id=""modalPartsLeadSection"" >
                        <div class=""header-form-title"">
                            <h3>Parts Lead Time</h3>
                        </div>

                            <div class=""row"">
                                <div class=""col-md-4"">
                                    <div><label><b>Time Option</b></label></div>
                                    <div><span id=""Modal-Time-Option"">{{Time-Option}}</span></div>
                                </div>
                                <div class=""col-md-4"">
                                    <div id=""time"" style=""{{Number-Style}}"">
                                        <div><label><b>Time Number</b></label></div>
                                        <div><span id=""Modal-Time-Number"">{{Time-Number}}</span></div>
                                    </div>
                                    <div>
                                        <div class=""row"" style=""{{Range-Style}}"" id=""modalPartsRange"">
                                            <div class=""col-md-6"">
                                                <div><label><b>Time Range From</b></label></div>
                                                <div><span id=""Modal-Time-Range-From"">{{Time-Range-From}}</span></div>
                                            </div>
                                            <div class=""col-md-6"">
                                                <div><label><b>Time Range To</b></label></div>
                                                <div><span id=""Modal-Time-Range-To"">{{Time-Range-To}}</span></div>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <div class=""col-md-4"">
                                    <div><label><b>Time Unit</b></label></div>
                                    <div><span id=""Modal-Time-Unit"">{{Time-Unit}}</span></div>
                                </div>
                            </div>
                    </div>";

            // Read HTML template files
            var folderName = Path.Combine("Resources", "HTMLTemplate");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fileName = "TableBody.text";
            var htmlTableTemplate = "TableTemplate.html";

            string tableBody = File.ReadAllText(Path.Combine(pathToSave, fileName));
            string htmlTemplate = File.ReadAllText(Path.Combine(pathToSave, htmlTableTemplate));

            // Generate rows for ProposedBreakouts
            string proposedBreakoutsRows = "";
            foreach (var jArray in quoteModel.ProposedBreakouts)
            {
                proposedBreakoutsRows += tableBody
                    .Replace("{{CostType}}", jArray.CostType)
                    .Replace("{{Description}}", jArray.Description)
                    .Replace("{{Rate}}", jArray.Rate.ToString())
                    .Replace("{{Quantity}}", jArray.Quantity.ToString())
                    .Replace("{{Techs}}", jArray.Techs.ToString())
                    .Replace("{{InitialCallSubTotal}}", jArray.InitialCallSubTotal.ToString());
            }

            // Generate rows for IncurredBreakouts if IsIncurredCost is true
            string incurredBreakoutsRows = "";
            string incurredTable = "";
            if (quoteModel.IsIncurredCost)
            {
                foreach (var jArray in quoteModel.IncurredBreakouts)
                {
                    incurredBreakoutsRows += tableBody
                        .Replace("{{CostType}}", jArray.CostType)
                        .Replace("{{Description}}", jArray.Description)
                        .Replace("{{Rate}}", jArray.Rate.ToString())
                        .Replace("{{Quantity}}", jArray.Quantity.ToString())
                        .Replace("{{Techs}}", jArray.Techs.ToString())
                        .Replace("{{InitialCallSubTotal}}", jArray.InitialCallSubTotal.ToString());
                }

                incurredTable = incurredTableTemplate
                    .Replace("{{incurredBreakoutsRows}}", incurredBreakoutsRows)
                    .Replace("{{IncurredSubTotal}}", quoteModel.IncurredTotals.IncurredSubTotal.ToString())
                    .Replace("{{IncurredTax}}", quoteModel.IncurredTotals.IncurredTax.ToString())
                    .Replace("{{IncurredInitialCallTotal}}", quoteModel.IncurredTotals.IncurredInitialCallTotal.ToString());
            }

            // Parts Lead Time Component
            if (quoteModel.TimeOption == "Number" && quoteModel.TimeUnit != "0")
            {
                partsLeadNumberTemplate = partsLeadNumberTemplate.Replace("{{Number-Style}}", "display: inline")
                                                                 .Replace("{{Range-Style}}", "display: none")
                                                                 .Replace("{{Time-Option}}", quoteModel.TimeOption)
                                                                 .Replace("{{Time-Number}}", quoteModel.TimeNumber.ToString())
                                                                 .Replace("{{Time-Unit}}", quoteModel.TimeUnit);
            }
            else if (quoteModel.TimeOption == "Range" && quoteModel.TimeRangeFrom != null)
            {
                partsLeadNumberTemplate = partsLeadNumberTemplate.Replace("{{Number-Style}}", "display:none")
                                                                 .Replace("{{Range-Style}}", "display:inline")
                                                                 .Replace("{{Time-Option}}", quoteModel.TimeOption)
                                                                 .Replace("{{Time-Range-From}}", quoteModel.TimeRangeFrom.ToString())
                                                                 .Replace("{{Time-Range-To}}", quoteModel.TimeRangeTo.ToString())
                                                                 .Replace("{{Time-Unit}}", quoteModel.TimeUnit);
            }
            else
            {
                partsLeadNumberTemplate = partsLeadNumberTemplate.Replace("{{PartsLead-Style}}", "display: none");
            }


            // Populate serviceTable with incurredTable or empty string
            string serviceTable = serviceTableTemplate
                .Replace("{{serviceDate}}", quoteModel.ServiceDate.ToString())
                .Replace("{{incurredTable}}", incurredTable);

            // Populate main HTML template
            string populatedHtml = htmlTemplate
                .Replace("{{poNumber}}", quoteModel.SMG_Vendor_PO)
                .Replace("{{date}}", quoteModel.Date.ToString())
                .Replace("{{client}}", quoteModel.SMG_CLIENT)
                .Replace("{{storeNumber}}", quoteModel.StoreNumber)
                .Replace("{{location}}", quoteModel.Location)
                .Replace("{{vendor}}", quoteModel.Vendor)
                .Replace("{{email}}", quoteModel.Email)
                .Replace("{{serviceRepName}}", quoteModel.ServiceRepName)
                .Replace("{{ProposedSubTotal}}", quoteModel.ProposedTotals.ProposedSubTotal.ToString())
                .Replace("{{ProposedTax}}", quoteModel.ProposedTotals.ProposedTax.ToString())
                .Replace("{{ProposedInitialCallTotal}}", quoteModel.ProposedTotals.ProposedInitialCallTotal.ToString())
                .Replace("{{message}}", quoteModel.Message)
                .Replace("{{disclaimer}}", quoteModel.Disclaimer)
                .Replace("{{servicesDescription}}", quoteModel.DescriptionOfServices)
                .Replace("{{proposedBreakoutsRows}}", proposedBreakoutsRows)
                .Replace("{{inccuredContainer}}", serviceTable)
                .Replace("{{PartsLeadContainer}}", partsLeadNumberTemplate);

            

            return populatedHtml;
        }

    }


}
