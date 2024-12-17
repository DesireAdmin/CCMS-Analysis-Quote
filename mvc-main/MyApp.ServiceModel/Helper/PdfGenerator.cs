using System;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Layout.Borders;
using iText.IO.Image;
using Microsoft.Extensions.Configuration;
using Azure.Storage.Blobs;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;
using MyApp.ServiceModel.DatabaseModel;
using System.IO;
using System.Diagnostics.Metrics;
using Azure.Storage.Blobs.Models;

namespace MyApp.ServiceModel.Helper
{
    public class PdfGenerator
    {

        private readonly IConfiguration _configuration;
        private string connectionString { get; set; }
        private string containerName { get; set; }
        private static string blobName = "output.pdf"; // Name for the PDF in Blob Storage

        public PdfGenerator(IConfiguration _configuration)
        {
            connectionString = _configuration.GetSection("AzureExplorer:PrimaryConnectionString").Value;
            containerName = _configuration.GetSection("AzureExplorer:ContainerName2").Value;

        }
        public async Task<MemoryStream> GeneratePdf(QuoteModel quoteModel)
        {
            try
            {
                MemoryStream memoryStream = new MemoryStream(); // Do not use `using` here

                // Create a PdfWriter
                PdfWriter writer = new PdfWriter(memoryStream, new WriterProperties().SetCompressionLevel(CompressionConstants.BEST_COMPRESSION)); // Prevent automatic stream closure
                PdfDocument pdf = new PdfDocument(writer);
                Document document = new Document(pdf);

                #region Title
                // Add a table with 2 columns to hold the logo and the title
                float[] columnWidths = { 1, 5 }; // Adjust column widths as needed
                Table logoAndTitleTable = new Table(columnWidths).UseAllAvailableWidth();
                logoAndTitleTable.SetBorder(Border.NO_BORDER); // Remove table borders

                var folderName = Path.Combine("wwwroot", "img");
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), folderName);
                var fileName = "SMGFacilitiesLogo.jpg";

                string logoPath = Path.Combine(folderPath, fileName);
                //D:\DesireInfoweb\Bradly\Master\mvc - main\MyApp\wwwroot\img\SMGFacilitiesLogo.jpg

                // Add the logo to the first cell
                ImageData logoData = ImageDataFactory.Create(logoPath);
                Image logo = new Image(logoData).ScaleToFit(50, 50);
                logoAndTitleTable.AddCell(new Cell()
                    .Add(logo)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)); // Align logo vertically

                // Add the title to the second cell
                Paragraph heading = new Paragraph("SMG Facilities Proposal Form Summary")
                    .SetTextAlignment(TextAlignment.LEFT) // Align to the left of the cell
                    .SetFontSize(24)
                    .SimulateBold(); // Make it bold
                logoAndTitleTable.AddCell(new Cell()
                    .Add(heading)
                    .SetBorder(Border.NO_BORDER)
                    .SetVerticalAlignment(VerticalAlignment.MIDDLE)); // Align title vertically

                // Add the table to the document
                document.Add(logoAndTitleTable);

                // Add some spacing below
                document.Add(new Paragraph("\n"));

                #endregion

                // Adding key-value pairs as Paragraphs
                AddKeyValueParagraph("SMG Vendor PO # :", quoteModel.SMG_Vendor_PO, document);
                AddKeyValueParagraph("Date :", quoteModel.Date.ToString("dd-MM-yyyy"), document);
                AddKeyValueParagraph("Store Name :", quoteModel.SMG_CLIENT, document);
                AddKeyValueParagraph("Store # :", quoteModel.StoreNumber, document);
                AddKeyValueParagraph("Address :", quoteModel.Location, document);
                AddKeyValueParagraph("Vendor :", quoteModel.Vendor, document);
                AddKeyValueParagraph("Email :", quoteModel.Email, document);
                AddKeyValueParagraph("Service Rep Name :", quoteModel.ServiceRepName, document);

                // Add some spacing
                document.Add(new Paragraph("\n"));
                document.Add(PrintIsIncurredCost(quoteModel));

                #region Breakout Tables
                // Add the Initial Breakout table to the document
                Table initialCallTable = FillInitialCallTable(quoteModel);
                if (initialCallTable != null)
                {
                    // Add "INITIAL CALL BREAKOUT" section
                    Paragraph initialCallHeading = new Paragraph("INITIAL CALL BREAKOUT")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(14)
                        .SimulateBold()
                        .SetFontColor(ColorConstants.DARK_GRAY);
                    Paragraph incurredIssueDescriptionHeading = new Paragraph("Issue(s) discovered while on-site & Description of Work Performed :")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SimulateBold()
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetPaddingTop(5);
                    Paragraph incurredIssueDescription = new Paragraph(quoteModel.IncurredIssueDescription)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetPaddingBottom(5);

                    document.Add(initialCallHeading);
                    document.Add(incurredIssueDescriptionHeading);
                    document.Add(incurredIssueDescription);

                    document.Add(initialCallTable);
                    // Add some spacing
                    document.Add(new Paragraph("\n"));
                }

                // Add the proposed call breakout table to the document
                Table proposedCallTable = FillProposedCallTable(quoteModel);
                if (proposedCallTable != null)
                {
                    // Add "INITIAL CALL BREAKOUT" section
                    Paragraph proposedCallHeading = new Paragraph("SUMMARY OF PROPOSED QUOTED WORK")
                        .SetTextAlignment(TextAlignment.CENTER)
                        .SetFontSize(14)
                        .SimulateBold()
                        .SetFontColor(ColorConstants.DARK_GRAY); // Subtle color for heading

                    Paragraph proposedIssueDescriptionHeading = new Paragraph("Detailed description of proposed work to address unresolved issues. :")
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SimulateBold()
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetPaddingTop(5);
                    Paragraph proposedIssueDescription = new Paragraph(quoteModel.ProposedIssueDescription)
                        .SetTextAlignment(TextAlignment.LEFT)
                        .SetFontSize(12)
                        .SetFontColor(ColorConstants.DARK_GRAY)
                        .SetPaddingBottom(5);

                    document.Add(proposedCallHeading);
                    document.Add(proposedIssueDescriptionHeading);
                    document.Add(proposedIssueDescription);

                    document.Add(proposedCallTable);
                }
                #endregion

                // Add some spacing
                document.Add(new Paragraph("\n"));

                // Add the personal details table to the document
                Table descriptionofServicesTable = FillDescriptionofServicesTable(quoteModel);
                //if (descriptionofServicesTable != null)
                //{
                    document.Add(descriptionofServicesTable);
                //}

                if (!string.IsNullOrEmpty(quoteModel.TimeOption))
                {
                    Table partsLeadTimeTable = FillPartsLeadTime(quoteModel);
                    if (partsLeadTimeTable != null)
                    {
                        // Add some spacing
                        document.Add(new Paragraph("\n"));

                        // Add "Parts Lead Time" section
                        Paragraph partsLeadTimeHeading = new Paragraph("Parts Lead Time")
                            .SetTextAlignment(TextAlignment.CENTER)
                            .SetFontSize(18)
                            .SimulateBold()
                            .SetFontColor(ColorConstants.DARK_GRAY); // Subtle color for heading
                        document.Add(partsLeadTimeHeading);

                        document.Add(partsLeadTimeTable);
                    }
                }
                

                // Finalize the document
                document.Close();

                // Upload the PDF to Azure Blob Storage
                await UploadPdfToAzureBlobAsync(memoryStream.ToArray(), quoteModel.Id, blobName);

                //// Reset the memory stream position to the beginning
                //memoryStream.Position = 0;

                MemoryStream readMemoryStream = await ReadPdfFromAzureBlobAsync(quoteModel.Id, blobName);
                return readMemoryStream;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occured while generating pdf : {ex.Message}");
                return null;
            }
        }

        private Table FillPartsLeadTime(QuoteModel quoteModel)
        {
            if (quoteModel.TimeNumber == null && quoteModel.TimeOption == "Number" || quoteModel.TimeOption == "Range" && quoteModel.TimeRangeFrom == null)
            {
                return null; // Return null if conditions are not met
            }

            // Determine the number of columns
            int columnCount = quoteModel.TimeOption == "Range" && quoteModel.TimeRangeFrom != null ? 4 : 3;

            // Create the table and set no borders
            Table partsLeadTimeTable = new Table(columnCount)
                .UseAllAvailableWidth()
                .SetBorder(Border.NO_BORDER)
                .SetFontSize(12);

            // Add headers with no borders
            partsLeadTimeTable.AddCell(new Paragraph("Time Option")
                .SimulateBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetPaddingBottom(5))
                .SetBorder(Border.NO_BORDER);

            if (columnCount == 4)
            {
                partsLeadTimeTable.AddCell(new Paragraph("Time Range From")
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);

                partsLeadTimeTable.AddCell(new Paragraph("Time Range To")
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);
            }
            else
            {
                partsLeadTimeTable.AddCell(new Paragraph("Time Number")
                    .SimulateBold()
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);
            }

            partsLeadTimeTable.AddCell(new Paragraph("Time Unit")
                .SimulateBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetPaddingBottom(5))
                .SetBorder(Border.NO_BORDER);

            // Add row data with no borders
            partsLeadTimeTable.AddCell(new Paragraph(quoteModel.TimeOption)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetPaddingBottom(5))
                .SetBorder(Border.NO_BORDER);

            if (quoteModel.TimeOption == "Range")
            {
                partsLeadTimeTable.AddCell(new Paragraph(quoteModel.TimeRangeFrom.ToString())
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);

                partsLeadTimeTable.AddCell(new Paragraph(quoteModel.TimeRangeTo.ToString())
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);
            }
            else
            {
                partsLeadTimeTable.AddCell(new Paragraph(quoteModel.TimeNumber.ToString())
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetPaddingTop(5)
                    .SetPaddingBottom(5))
                    .SetBorder(Border.NO_BORDER);
            }

            partsLeadTimeTable.AddCell(new Paragraph(quoteModel.TimeUnit)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetPaddingTop(5)
                .SetPaddingBottom(5))
                .SetBorder(Border.NO_BORDER);

            return partsLeadTimeTable;
        }

        private Table FillDescriptionofServicesTable(QuoteModel quoteModel)
        {
            // Create a table with 2 columns for Message, Disclaimer, and Description of Services
            Table messageTable = new Table(2).SetBorder(Border.NO_BORDER).UseAllAvailableWidth(); // 2 columns, full width

            // Add the "Message" section (first row)
            //messageTable.AddCell(new Paragraph("Message")
            //    .SetTextAlignment(TextAlignment.LEFT)
            //    .SetFontSize(13)
            //    .SetBorder(Border.NO_BORDER)
            //    .SimulateBold());
            //messageTable.AddCell(new Paragraph(quoteModel.Message)
            //    .SetTextAlignment(TextAlignment.LEFT)
            //    .SetFontSize(12)
            //    .SetBorder(Border.NO_BORDER)
            //    .SetFontColor(ColorConstants.BLACK));

            // Add the "Disclaimer" section (second row)
            if(quoteModel.Disclaimer != null)
            {
                messageTable.AddCell(new Paragraph("Disclaimer")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetBorder(Border.NO_BORDER)
                .SetFontSize(13)
                .SimulateBold());
                messageTable.AddCell(new Paragraph(quoteModel.Disclaimer)
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetFontSize(12)
                    .SetBorder(Border.NO_BORDER)
                    .SetFontColor(ColorConstants.BLACK));
            }
            

            // Add the "Description of Services" section (third row)
            //messageTable.AddCell(new Paragraph("Description of Services:")
            //    .SetTextAlignment(TextAlignment.LEFT)
            //    .SetFontSize(13)
            //    .SetBorder(Border.NO_BORDER)
            //    .SimulateBold());
            //messageTable.AddCell(new Paragraph(quoteModel.DescriptionOfServices)
            //    .SetTextAlignment(TextAlignment.LEFT)
            //    .SetFontSize(12)
            //    .SetBorder(Border.NO_BORDER)
            //    .SetFontColor(ColorConstants.BLACK));

            return messageTable;
        }

        private Table FillProposedCallTable(QuoteModel quoteModel)
        {
            if (quoteModel.ProposedBreakouts == null)
            {
                return null; // Return null if quoteModel is null
            }

            // Create a table with 6 columns for initial call breakout
            Table proposedCallTable = new Table(6).UseAllAvailableWidth().SetBorder(Border.NO_BORDER).SetFontSize(12); // Make table full width

            // Add header cells to the table
            proposedCallTable.AddHeaderCell("Cost Type")
                .SetTextAlignment(TextAlignment.CENTER);
            proposedCallTable.AddHeaderCell("Description")
                .SetTextAlignment(TextAlignment.CENTER);
            proposedCallTable.AddHeaderCell("#Techs")
                .SetTextAlignment(TextAlignment.CENTER);
            proposedCallTable.AddHeaderCell("Quantity")
                .SetTextAlignment(TextAlignment.CENTER);
            proposedCallTable.AddHeaderCell("Rate")
                .SetTextAlignment(TextAlignment.CENTER);
            proposedCallTable.AddHeaderCell("Quote Sub Total")
                .SetTextAlignment(TextAlignment.CENTER);

            foreach (var proposedBreakouts in quoteModel.ProposedBreakouts)
            {
                proposedCallTable.AddCell(proposedBreakouts.CostType);
                proposedCallTable.AddCell(proposedBreakouts.Description);
                proposedCallTable.AddCell(proposedBreakouts.Techs.ToString());
                proposedCallTable.AddCell(proposedBreakouts.Quantity.ToString());
                proposedCallTable.AddCell(proposedBreakouts.Rate.ToString());
                proposedCallTable.AddCell(proposedBreakouts.InitialCallSubTotal.ToString());
            }

            // Add rows with colspan for Sub Total, Tax, and Total
            proposedCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            proposedCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Sub Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(5));
            proposedCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.ProposedTotals.ProposedSubTotal.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(5));

            // Repeat for Tax and Total rows
            proposedCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            proposedCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Tax")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER));
            proposedCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.ProposedTotals.ProposedTax.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            proposedCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            proposedCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Proposed Quote Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER));
            proposedCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.ProposedTotals.ProposedInitialCallTotal.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            return proposedCallTable;
        }

        private Table FillInitialCallTable(QuoteModel quoteModel)
        {
            if (quoteModel.IncurredBreakouts == null)
            {
                return null; // Return null if quoteModel is null
            }

            // Create a table with 6 columns for initial call breakout
            Table initialCallTable = new Table(6).UseAllAvailableWidth().SetBorder(Border.NO_BORDER).SetFontSize(12); // Make table full width

            // Add header cells to the table
            initialCallTable.AddHeaderCell("Cost Type")
                .SetTextAlignment(TextAlignment.CENTER);
            initialCallTable.AddHeaderCell("Description")
                .SetTextAlignment(TextAlignment.CENTER);
            initialCallTable.AddHeaderCell("#Techs")
                .SetTextAlignment(TextAlignment.CENTER);
            initialCallTable.AddHeaderCell("Quantity")
                .SetTextAlignment(TextAlignment.CENTER);
            initialCallTable.AddHeaderCell("Rate")
                .SetTextAlignment(TextAlignment.CENTER);
            initialCallTable.AddHeaderCell("Initial Call Sub Total")
                .SetTextAlignment(TextAlignment.CENTER);


            foreach (var incurredBreakout in quoteModel.IncurredBreakouts)
            {
                initialCallTable.AddCell(incurredBreakout.CostType);
                initialCallTable.AddCell(incurredBreakout.Description);
                initialCallTable.AddCell(incurredBreakout.Techs.ToString());
                initialCallTable.AddCell(incurredBreakout.Quantity.ToString());
                initialCallTable.AddCell(incurredBreakout.Rate.ToString());
                initialCallTable.AddCell(incurredBreakout.InitialCallSubTotal.ToString());
            }

            initialCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            initialCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Sub Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(5));
            initialCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.IncurredTotals.IncurredSubTotal.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER)
                .SetPaddingTop(5));


            initialCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            initialCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Tax")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER));
            initialCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.IncurredTotals.IncurredTax.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            initialCallTable.AddCell(new Cell(1, 3).SetBorder(Border.NO_BORDER));
            initialCallTable.AddCell(new Cell(1, 2).Add(new Paragraph("Initial Call Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold())
                .SetBorder(Border.NO_BORDER));
            initialCallTable.AddCell(new Cell(1, 1).Add(new Paragraph(quoteModel.IncurredTotals.IncurredInitialCallTotal.ToString())
                .SetTextAlignment(TextAlignment.LEFT))
                .SetBorder(Border.NO_BORDER));

            return initialCallTable;
        }

        // Helper method to add key-value pair as a Paragraph
        private void AddKeyValueParagraph(string key, string value, Document document)
        {
            Paragraph paragraph = new Paragraph();

            // Add bold key
            paragraph.Add(new Text(key).SimulateBold());

            // Add the value (without bold)
            paragraph.Add(new Text(" " + value));

            // Add the complete paragraph to the document
            document.Add(paragraph.SetFontColor(ColorConstants.BLACK).SetPadding(2));

        }

        private async Task UploadPdfToAzureBlobAsync(byte[] pdfStream, string id, string blobName)
        {
            // Create a BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get the container reference
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            // Get the blob reference
            BlobClient blobClient = containerClient.GetBlobClient($"{id}_{blobName}");

            // Upload the PDF data as a blob
            await blobClient.UploadAsync(new MemoryStream(pdfStream), overwrite: true);

            Console.WriteLine("PDF uploaded to Azure Blob Storage.");
        }

        private async Task<MemoryStream> ReadPdfFromAzureBlobAsync(string id, string blobName)
        {
            // Create a BlobServiceClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);

            // Get the container reference
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Get the blob reference
            BlobClient blobClient = containerClient.GetBlobClient($"{id}_{blobName}");

            // Check if the blob exists
            if (await blobClient.ExistsAsync())
            {
                // Download the blob content to a MemoryStream
                BlobDownloadInfo download = await blobClient.DownloadAsync();

                // Create a MemoryStream and copy the downloaded data into it
                MemoryStream memoryStream = new MemoryStream();
                await download.Content.CopyToAsync(memoryStream);

                // Set the position of the memory stream to the beginning for reading
                memoryStream.Position = 0;

                Console.WriteLine("PDF readed from Azure Blob Storage.");
                return memoryStream;
            }
            else
            {
                Console.WriteLine("Blob does not exist.");
                return null;
            }
        }

        private Paragraph PrintIsIncurredCost(QuoteModel quoteModel)
        {
            // Create the bold portion of the text
            // Create bold text for "Incurred Cost? :"
            Text boldText = new Text("Incurred Cost? : ").SimulateBold().SetFontColor(ColorConstants.DARK_GRAY);
            Text normalText;
            if (quoteModel.IncurredBreakouts != null) {

                normalText = new Text("Yes").SetFontColor(ColorConstants.DARK_GRAY);
            }
            else
            {
                normalText = new Text("No").SetFontColor(ColorConstants.DARK_GRAY);
            }

            // Combine the bold and unbold text into a single paragraph
            Paragraph isIncurredCost = new Paragraph()
                .Add(boldText)
                .Add(normalText)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(12);

            return isIncurredCost;
        }

    }
}
