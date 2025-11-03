using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net.Mail;
using System.Net;

namespace Api.Infrastructure.Utilities.Email;

public class SmtpEmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<SmtpEmailService> _logger;

    public SmtpEmailService(IConfiguration configuration, ILogger<SmtpEmailService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task SendEmailAsync(string to, string subject, string body)
    {
        var host = _configuration.GetValue<string>("Smtp:Host");
        var port = _configuration.GetValue<int?>("Smtp:Port") ?? 25;
        var username = _configuration.GetValue<string>("Smtp:Username");
        var password = _configuration.GetValue<string>("Smtp:Password");
        var from = _configuration.GetValue<string>("Smtp:From") ?? username;
        var enableSsl = _configuration.GetValue<bool?>("Smtp:EnableSsl") ?? true;

        if (string.IsNullOrWhiteSpace(host))
            throw new InvalidOperationException("SMTP host is not configured (Smtp:Host).");
        if (string.IsNullOrWhiteSpace(from))
            throw new InvalidOperationException("SMTP 'From' address is not configured (Smtp:From or Smtp:Username).");
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            _logger.LogWarning("SMTP username or password is empty. Sending may fail if the server requires authentication.");

        using var client = new SmtpClient(host, port)
        {
            EnableSsl = enableSsl,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(username, password),
            DeliveryMethod = SmtpDeliveryMethod.Network
        };

        using var msg = new MailMessage(from, to, subject ?? string.Empty, body ?? string.Empty);
        msg.IsBodyHtml = false;

        try
        {
            await client.SendMailAsync(msg);
            _logger.LogInformation("SMTP message sent to {To} via {Host}:{Port}", to, host, port);
        }
        catch (System.Exception ex)
        {
            // Log full exception for diagnosis and rethrow so callers may handle it
            _logger.LogError(ex, "Failed to send SMTP email to {To} via {Host}:{Port}", to, host, port);
            throw;
        }
    }
}
