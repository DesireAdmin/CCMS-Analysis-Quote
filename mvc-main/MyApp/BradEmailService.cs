using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Http;

namespace MyApp
{

    public interface IBradEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message, MemoryStream pdfAttachment, IFormFile file);
        string returnHtmlBody();
    }

    // This class is used by the application to send email for account confirmation and password reset.
    // For more details see https://go.microsoft.com/fwlink/?LinkID=532713
    public class BradEmailService : IBradEmailSender
    {
        public readonly IConfiguration _config;
        public BradEmailService(IConfiguration configurationManager)
        {
            _config = configurationManager;
        }

        public async Task SendEmailAsync(string email, string subject, string message, MemoryStream pdfAttachment, IFormFile file)
        {
            using (MailMessage mail = new MailMessage())
            {
                mail.From = new MailAddress(_config["SmtpConfig:Username"]);
                mail.To.Add(email);
                mail.Subject = subject;
                mail.Body = message;
                mail.IsBodyHtml = true;
                var attachment = new Attachment(pdfAttachment, "GeneratedDocument.pdf", "application/pdf");
                mail.Attachments.Add(attachment);

                var memoryStream = new MemoryStream();

                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;
                var attachmentClient = new Attachment(memoryStream, file.FileName, file.ContentType);
                mail.Attachments.Add(attachmentClient);




                //mail.Attachments.Add(new Attachment("D:\\TestFile.txt"));//--Uncomment this to send any attachment  
                using (SmtpClient smtp = new SmtpClient(_config["SmtpConfig:Host"], 587))
                {
                    smtp.Credentials = new NetworkCredential(_config["SmtpConfig:Username"], _config["SmtpConfig:Password"]);
                    smtp.EnableSsl = true;
                    smtp.Send(mail);
                }
            }

            return;
        }

        public string returnHtmlBody()
        {
            //string FilePath = "..\\MyApp.Common\\HTMLTemplate\\sendEmailNotFillAnswer.html";
            var folderName = Path.Combine("Resources", "HTMLTemplate");
            var pathToSave = Path.Combine(Directory.GetCurrentDirectory(), folderName);
            var fileName = "BradQuoteform.html";

            string actualFilePath = Path.Combine(pathToSave, fileName);
            if (File.Exists(actualFilePath))
            {
                using (StreamReader streamReader = new StreamReader(actualFilePath))
                {

                    string html = streamReader.ReadToEnd();
                    streamReader.Close();
                    return html;
                }
            }

            return "";
        }
    }
}
