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
                using (MemoryStream memoryStream = new MemoryStream())
                {
                    // Create a PdfWriter
                    using (PdfWriter writer = new PdfWriter(memoryStream))
                    {
                        // Create a PdfDocument
                        using (PdfDocument pdf = new PdfDocument(writer))
                        {
                            // Create a Document to add content
                            Document document = new Document(pdf);

                            #region Title
                            // Add a table with 2 columns to hold the logo and the title
                            float[] columnWidths = { 1, 5 }; // Adjust column widths as needed
                            Table logoAndTitleTable = new Table(columnWidths).UseAllAvailableWidth();
                            logoAndTitleTable.SetBorder(Border.NO_BORDER); // Remove table borders


                            //string binPath = AppDomain.CurrentDomain.BaseDirectory;
                            //var folderName = Path.Combine("wwwroot", "img");
                            //var Path = Path.Combine(folderName,"logo.svg");

                            // Add the logo to the first cell
                            ImageData logoData = ImageDataFactory.Create("D:\\DesireInfoweb\\Bradly\\Master\\mvc-main\\MyApp\\wwwroot\\img\\SMGFacilitiesLogo.jpg"); // Replace with your logo file path
                            Image logo = new Image(logoData).ScaleToFit(50, 50); // Scale the logo to fit
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

                            // Add some spacing
                            document.Add(new Paragraph("\n"));

                            // Add the personal details table to the document
                            Table personalDetailsTable = FillPersonalDetailsTable(quoteModel);
                            if (personalDetailsTable != null)
                            {
                                document.Add(personalDetailsTable);
                            }

                            #region Breakout Tables
                            // Add some spacing
                            document.Add(new Paragraph("\n"));

                            // Add "INITIAL CALL BREAKOUT" section
                            Paragraph initialCallHeading = new Paragraph("INITIAL CALL BREAKOUT")
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetFontSize(18)
                                .SimulateBold()
                                .SetFontColor(ColorConstants.DARK_GRAY); // Subtle color for heading
                            document.Add(initialCallHeading);

                            // Add the Initial Breakout table to the document
                            Table initialCallTable = FillInitialCallTable(quoteModel);
                            if (initialCallTable != null)
                            {
                                document.Add(initialCallTable);
                            }

                            // Add some spacing
                            document.Add(new Paragraph("\n"));

                            // Add "INITIAL CALL BREAKOUT" section
                            Paragraph proposedCallHeading = new Paragraph("SUMMARY OF PROPOSED QUOTED WORK")
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetFontSize(18)
                                .SimulateBold()
                                .SetFontColor(ColorConstants.DARK_GRAY); // Subtle color for heading
                            document.Add(proposedCallHeading);

                            // Add the proposed call breakout table to the document
                            Table proposedCallTable = FillProposedCallTable(quoteModel);
                            if (proposedCallTable != null)
                            {
                                document.Add(proposedCallTable);
                            }
                            #endregion

                            #region Description of Services
                            // Add some spacing
                            document.Add(new Paragraph("\n"));

                            // Add the personal details table to the document
                            Table descriptionofServicesTable = FillDescriptionofServicesTable(quoteModel);
                            if (descriptionofServicesTable != null)
                            {
                                document.Add(descriptionofServicesTable);
                            }
                            #endregion

                            #region Parts Lead
                            // Add some spacing
                            document.Add(new Paragraph("\n"));

                            // Add "Parts Lead Time" section
                            Paragraph partsLeadTimeHeading = new Paragraph("Parts Lead Time")
                                .SetTextAlignment(TextAlignment.LEFT)
                                .SetFontSize(18)
                                .SimulateBold()
                                .SetFontColor(ColorConstants.DARK_GRAY); // Subtle color for heading
                            document.Add(partsLeadTimeHeading);

                            Table partsLeadTimeTable = FillPartsLeadTime(quoteModel);
                            if (partsLeadTimeTable != null)
                            {
                                document.Add(partsLeadTimeTable);
                            }
                            #endregion

                            // Finalize the document
                            document.Close();
                        }
                    }

                    // Reset the memory stream position to the beginning
                    memoryStream.Position = 0;

                    // Upload the PDF to Azure Blob Storage
                    await UploadPdfToAzureBlobAsync(memoryStream.ToArray(), quoteModel.Id, blobName);
                    return memoryStream;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error Occured while generating pdf : {ex.Message}");
                return null;
            }
        }

        private Table FillPartsLeadTime(QuoteModel quoteModel)
        {
            // Create a table with 2 columns for Parts Lead Time
            float[] partsLeadTimeTableColumnWidths = { 2, 8 };
            Table partsLeadTimeTable = new Table(partsLeadTimeTableColumnWidths).UseAllAvailableWidth(); // Make table full width

            // Define key-value pairs for Parts Lead Time
            string[] partsKeys = new string[] { "Time Option:", "Time Number:", "Time Unit:" };
            string[] partsValues = new string[] { "Express", "5", "Days" };

            // Add key-value pairs to the table with bold keys and regular values
            for (int i = 0; i < partsKeys.Length; i++)
            {
                // Key (Bold and Dark Gray)
                partsLeadTimeTable.AddCell(new Paragraph(partsKeys[i])
                    .SetFontSize(13)
                    .SimulateBold()
                    .SetFontColor(ColorConstants.DARK_GRAY)
                    .SetPadding(5)); // Add padding inside the cell

                // Value (Regular text, with left alignment)
                partsLeadTimeTable.AddCell(new Paragraph(partsValues[i])
                    .SetTextAlignment(TextAlignment.LEFT)
                    .SetPadding(5)); // Add padding inside the cell
            }
            return partsLeadTimeTable;
        }

        private Table FillDescriptionofServicesTable(QuoteModel quoteModel)
        {
            // Create a table with 2 columns for Message, Disclaimer, and Description of Services
            Table messageTable = new Table(2).UseAllAvailableWidth(); // 2 columns, full width
            messageTable.SetBorder(Border.NO_BORDER);  // Set border to none for the entire table

            // Add the "Message" section (first row)
            messageTable.AddCell(new Paragraph("Message")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(13)
                .SimulateBold());
            messageTable.AddCell(new Paragraph(quoteModel.Message)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.BLACK));

            // Add the "Disclaimer" section (second row)
            messageTable.AddCell(new Paragraph("Disclaimer")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(13)
                .SimulateBold());
            messageTable.AddCell(new Paragraph(quoteModel.Disclaimer)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.BLACK));

            // Add the "Description of Services" section (third row)
            messageTable.AddCell(new Paragraph("Description of Services:")
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(13)
                .SimulateBold());
            messageTable.AddCell(new Paragraph(quoteModel.DescriptionOfServices)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetFontSize(12)
                .SetFontColor(ColorConstants.BLACK));

            return messageTable;
        }

        private Table FillProposedCallTable(QuoteModel quoteModel)
        {
            // Create a table with 6 columns for initial call breakout
            Table proposedCallTable = new Table(6).UseAllAvailableWidth(); // Make table full width

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
            proposedCallTable.AddCell(new Paragraph("Sub Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold()); // Use SimulateBold instead of SetBold
            proposedCallTable.AddCell(new Paragraph(quoteModel.ProposedTotals.ProposedSubTotal.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            // Repeat for Tax and Total rows
            proposedCallTable.AddCell(new Paragraph("Tax")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold());
            proposedCallTable.AddCell(new Paragraph(quoteModel.ProposedTotals.ProposedTax.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            proposedCallTable.AddCell(new Paragraph("Proposed Quote Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold());
            proposedCallTable.AddCell(new Paragraph(quoteModel.ProposedTotals.ProposedInitialCallTotal.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            return proposedCallTable;
        }

        private Table FillInitialCallTable(QuoteModel quoteModel)
        {
            // Create a table with 6 columns for initial call breakout
            Table initialCallTable = new Table(6).UseAllAvailableWidth(); // Make table full width

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

            // Add rows with colspan for Sub Total, Tax, and Total
            initialCallTable.AddCell(new Paragraph("Sub Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold()); // Use SimulateBold instead of SetBold
            initialCallTable.AddCell(new Paragraph(quoteModel.IncurredTotals.IncurredSubTotal.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            // Repeat for Tax and Total rows
            initialCallTable.AddCell(new Paragraph("Tax")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold());
            initialCallTable.AddCell(new Paragraph(quoteModel.IncurredTotals.IncurredTax.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            initialCallTable.AddCell(new Paragraph("Initial Call Total")
                .SetTextAlignment(TextAlignment.LEFT)
                .SimulateBold());
            initialCallTable.AddCell(new Paragraph(quoteModel.IncurredTotals.IncurredInitialCallTotal.ToString())
                .SetTextAlignment(TextAlignment.CENTER));

            return initialCallTable;
        }

        private Table FillPersonalDetailsTable(QuoteModel quoteModel)
        {
            // Create a table with 2 columns for personal details
            Table personalDetailsTable = new Table(2).UseAllAvailableWidth(); // Make table full width

            personalDetailsTable.AddCell(new Paragraph("SMG Vendor PO #")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.SMG_Vendor_PO)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Date")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.Date.ToString("dd-MM-yyyy"))
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("SMG Client")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.SMG_CLIENT)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Store #")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.StoreNumber)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Location")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.Location)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Vendor")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.Vendor)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Email")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.Email)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            personalDetailsTable.AddCell(new Paragraph("Service Rep Name")
                .SimulateBold()
                .SetFontColor(ColorConstants.BLACK)
                .SetPadding(5));
            personalDetailsTable.AddCell(new Paragraph(quoteModel.ServiceRepName)
                .SetTextAlignment(TextAlignment.LEFT)
                .SetPadding(5));

            return personalDetailsTable;
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

    }
}
