using FluentEmail.Core;
using Pento.Application.Abstractions.External.Email;

namespace Pento.Infrastructure.External.Email;

internal sealed class EmailService(IFluentEmail fluentEmail) : IEmailService
{
    public async Task SendAsync(string recipient, string subject, string body)
    {
        await fluentEmail
            .To(recipient)
            .Subject(subject)
            .Body(body, isHtml: true)
            .SendAsync();
    }
}
