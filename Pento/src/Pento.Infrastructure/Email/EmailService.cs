using Pento.Application.Abstractions.Email;

namespace Pento.Infrastructure.Email;

internal sealed class EmailService : IEmailService
{
    public Task SendAsync(string recipient, string subject, string body)
    {
        throw new NotImplementedException();
    }
}
