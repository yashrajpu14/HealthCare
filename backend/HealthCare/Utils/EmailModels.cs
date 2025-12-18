namespace HealthCare.Utils;

public sealed class EmailRequest
{
    public required string To { get; init; }
    public string? ToName { get; init; }

    public required string Subject { get; init; }

    // Send either HtmlBody OR (TemplateKey + Variables)
    public string? HtmlBody { get; init; }

    public string? TemplateKey { get; init; }   // e.g. "verify_account", "reset_password"
    public Dictionary<string, string>? Variables { get; init; }

    public bool SaveToSentItems { get; init; } = true;
}
