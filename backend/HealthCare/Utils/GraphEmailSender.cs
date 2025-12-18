using Azure.Identity;
using HealthCare.Utils.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace HealthCare.Utils;

public sealed class GraphEmailSender : IEmailSender
{
    private readonly GraphServiceClient _graph;

    public GraphEmailSender(IConfiguration config)
    {
        var clientId = config["MicrosoftGraph:ClientId"];
        var tenantId = config["MicrosoftGraph:TenantId"] ?? "consumers";

        if (string.IsNullOrWhiteSpace(clientId))
            throw new InvalidOperationException("MicrosoftGraph:ClientId is missing in appsettings.json");

        var scopes = new[] { "User.Read", "Mail.Send" };

        var credential = new DeviceCodeCredential(new DeviceCodeCredentialOptions
        {
            ClientId = clientId,
            TenantId = tenantId,
            DeviceCodeCallback = (code, ct) =>
            {
                Console.WriteLine(code.Message);
                return Task.CompletedTask;
            }
        });

        _graph = new GraphServiceClient(credential, scopes);
    }

    public async Task SendAsync(EmailRequest req, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(req.To))
            throw new ArgumentException("EmailRequest.To is required");

        // Build subject + html from direct values or template
        var subject = req.Subject ?? "";
        var htmlBody = req.HtmlBody ?? "";

        if (!string.IsNullOrWhiteSpace(req.TemplateKey))
        {
            var vars = req.Variables ?? new Dictionary<string, string>();
            var rendered = EmailTemplates.Render(req.TemplateKey, vars); // uses html-encoding inside :contentReference[oaicite:2]{index=2}
            if (string.IsNullOrWhiteSpace(subject)) subject = rendered.Subject;
            if (string.IsNullOrWhiteSpace(htmlBody)) htmlBody = rendered.Html;
        }

        if (string.IsNullOrWhiteSpace(subject))
            throw new ArgumentException("Email subject is required");

        if (string.IsNullOrWhiteSpace(htmlBody))
            throw new ArgumentException("Email HTML body is required (HtmlBody or TemplateKey).");

        var message = new Message
        {
            Subject = subject,
            Body = new ItemBody { ContentType = BodyType.Html, Content = htmlBody },
            ToRecipients = new List<Recipient>
            {
                new Recipient
                {
                    EmailAddress = new EmailAddress
                    {
                        Address = req.To,
                        Name = req.ToName
                    }
                }
            }
        };

        await _graph.Me.SendMail.PostAsync(
            new Microsoft.Graph.Me.SendMail.SendMailPostRequestBody
            {
                Message = message,
                SaveToSentItems = req.SaveToSentItems
            },
            cancellationToken: ct
        );
    }
}
