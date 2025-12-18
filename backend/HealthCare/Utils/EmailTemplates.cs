using System.Net;

namespace HealthCare.Utils;


public static class EmailTemplates
{
    // Simple string templates with {{placeholders}}
    private static readonly Dictionary<string, (string Subject, string Html)> Templates = new()
    {
        ["verify_account"] = (
            Subject: "Verify your account",
            Html:
            """
        <div style="font-family:Arial,sans-serif;max-width:560px">
          <h2>Verify your account</h2>
          <p>Hi {{name}},</p>
          <p>Please verify your account by clicking the button below:</p>
          <p><a href="{{verifyUrl}}" style="display:inline-block;padding:10px 16px;background:#111;color:#fff;text-decoration:none;border-radius:8px">Verify Account</a></p>
          <p>If you didn’t request this, ignore this email.</p>
          <hr/>
          <p style="color:#666;font-size:12px">This link expires in {{expiresIn}}.</p>
        </div>
        """
        ),

        ["reset_password"] = (
            Subject: "Reset your password",
            Html:
            """
        <div style="font-family:Arial,sans-serif;max-width:560px">
          <h2>Reset password</h2>
          <p>Hi {{name}},</p>
          <p>We received a request to reset your password.</p>
          <p><a href="{{resetUrl}}" style="display:inline-block;padding:10px 16px;background:#111;color:#fff;text-decoration:none;border-radius:8px">Reset Password</a></p>
          <p>If you didn’t request this, you can safely ignore this email.</p>
          <hr/>
          <p style="color:#666;font-size:12px">This link expires in {{expiresIn}}.</p>
        </div>
        """
        ),

        ["password_changed"] = (
            Subject: "Your password was changed",
            Html:
            """
        <div style="font-family:Arial,sans-serif;max-width:560px">
          <h2>Password changed</h2>
          <p>Hi {{name}},</p>
          <p>Your password was successfully changed on {{changedAt}}.</p>
          <p>If this wasn’t you, secure your account immediately.</p>
        </div>
        """
        )
    };

    public static (string Subject, string Html) Render(string templateKey, Dictionary<string, string> vars)
    {
        if (!Templates.TryGetValue(templateKey, out var tpl))
            throw new ArgumentException($"Unknown email template key: {templateKey}");

        string subject = ReplaceTokens(tpl.Subject, vars);
        string html = ReplaceTokens(tpl.Html, vars);

        return (subject, html);
    }

    private static string ReplaceTokens(string input, Dictionary<string, string> vars)
    {
        foreach (var kv in vars)
        {
            // HTML-encode values to avoid injection
            var safeValue = WebUtility.HtmlEncode(kv.Value ?? "");
            input = input.Replace("{{" + kv.Key + "}}", safeValue);
        }
        return input;
    }
}
