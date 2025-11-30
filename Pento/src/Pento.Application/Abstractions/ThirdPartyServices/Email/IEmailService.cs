namespace Pento.Application.Abstractions.ThirdPartyServices.Email;

public interface IEmailService
{
    Task SendAsync(string recipient, string subject, string body);
}
