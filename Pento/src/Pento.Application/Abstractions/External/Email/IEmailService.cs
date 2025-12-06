namespace Pento.Application.Abstractions.External.Email;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string body);
}
