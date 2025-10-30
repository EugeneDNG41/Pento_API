using FluentEmail.Core;
using Pento.Application.Abstractions.Caching;
using Pento.Application.Abstractions.Email;

namespace Pento.Infrastructure.Email;

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
