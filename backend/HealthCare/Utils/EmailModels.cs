namespace HealthCare.Utils;

public sealed class EmailRequest
{
    public string To { get; set; } = string.Empty;
    public string? ToName { get; set; }

    // Either provide Subject+HtmlBody OR use TemplateKey (+ Variables)
    public string? Subject { get; set; }
    public string? HtmlBody { get; set; }

    public string? TemplateKey { get; set; }                 // e.g., "verify_account"
    public Dictionary<string, string>? Variables { get; set; } // e.g., { name, verifyUrl, expiresIn }

    public bool SaveToSentItems { get; set; } = true;
}
