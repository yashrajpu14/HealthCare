namespace HealthCare.Utils.Interfaces;

public interface IEmailSender
{
    Task SendAsync(EmailRequest req, CancellationToken ct = default);
}
