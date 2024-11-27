using System.Net.Mail;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ServiceStack;
using ServiceStack.Jobs;

namespace MyApp.ServiceInterface;

/// <summary>
/// Configuration for sending emails using SMTP servers in EmailServices
/// </summary>
public class SmtpConfig
{
    public string Username { get; set; }
    public string Password { get; set; }
    public string Host { get; set; }
    public int Port { get; set; } = 587;
    public string FromEmail { get; set; }
    public string? FromName { get; set; }
    public string? DevToEmail { get; set; }
    public string? Bcc { get; set; }
}

public class SendEmail
{
    public string To { get; set; }
    public string? ToName { get; set; }
    public string Subject { get; set; }
    public string? BodyText { get; set; }
    public string? BodyHtml { get; set; }
}

[Worker("smtp")]
public class SendEmailCommand : SyncCommand<SendEmail>
{
    private static long count = 0;
    private readonly ILogger<SendEmailCommand> _logger;
    private readonly IBackgroundJobs _jobs;
    private readonly SmtpConfig _config;

    public SendEmailCommand(ILogger<SendEmailCommand> logger, IBackgroundJobs jobs, IOptions<SmtpConfig> config)
    {
        _logger = logger;
        _jobs = jobs;
        _config = config.Value;
    }

    protected override void Run(SendEmail request)
    {
        Interlocked.Increment(ref count);
        var log = Request.CreateJobLogger(_jobs, _logger);
        log.LogInformation("Sending {Count} email to {Email} with subject {Subject}", count, request.To, request.Subject);

        using var client = new SmtpClient(_config.Host, _config.Port)
        {
            Credentials = new System.Net.NetworkCredential(_config.Username, _config.Password),
            EnableSsl = true
        };

        var emailTo = _config.DevToEmail != null
            ? new MailAddress(_config.DevToEmail)
            : new MailAddress(request.To, request.ToName);

        var emailFrom = new MailAddress(_config.FromEmail, _config.FromName);

        var msg = new MailMessage(emailFrom, emailTo)
        {
            Subject = request.Subject,
            Body = request.BodyHtml ?? request.BodyText,
            IsBodyHtml = request.BodyHtml != null,
        };

        if (_config.Bcc != null)
        {
            msg.Bcc.Add(new MailAddress(_config.Bcc));
        }

        client.Send(msg);
    }
}
