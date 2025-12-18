using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using Microsoft.Graph.Models;

namespace HealthCare.Utils;

public sealed class GraphEmailSender
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
}
