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
        ),

        ["doctor_account_approved"] = (
            Subject: "Your Doctor Account Has Been Approved",
            Html:
            """
        <div style="font-family:Arial,sans-serif;max-width:560px;background:#f4f6f8;padding:16px;border-radius:10px">
          <div style="background:#ffffff;border-radius:10px;padding:22px;border:1px solid #e6e6e6">
            <h2 style="margin:0 0 12px;color:#1e88e5">Doctor Account Approved ✅</h2>

            <p>Hi <b>{{name}}</b>,</p>

            <p>
              Your doctor account has been approved after verifying your medical license.
            </p>

            <p>Your login credentials are:</p>

            <div style="background:#f9fafb;border:1px solid #e0e0e0;padding:14px;border-radius:8px;margin:14px 0">
              <p style="margin:6px 0"><b>Email:</b> {{email}}</p>
              <p style="margin:6px 0"><b>Temporary Password:</b> {{tempPassword}}</p>
            </div>

            <p style="margin:0 0 12px">
              For security reasons, please log in and <b>change your password immediately</b>.
            </p>

            <p style="margin:18px 0">
              <a href="{{loginUrl}}" style="display:inline-block;padding:10px 16px;background:#1e88e5;color:#fff;text-decoration:none;border-radius:8px">
                Login to Your Account
              </a>
            </p>

            <hr style="border:none;border-top:1px solid #eee;margin:18px 0" />

            <p style="color:#666;font-size:12px;margin:0">
              If you did not request this account or believe this email was sent in error,
              please contact support immediately.
            </p>
          </div>
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
