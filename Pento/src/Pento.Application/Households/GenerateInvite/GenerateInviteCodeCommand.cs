using FluentValidation;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GenerateInvite;

public sealed record GenerateInviteCodeCommand(Guid? HouseholdId, DateTime? CodeExpiration) : ICommand<string>;
internal sealed class GenerateInviteCodeCommandValidator : AbstractValidator<GenerateInviteCodeCommand>
{
    public GenerateInviteCodeCommandValidator()
    {
        RuleFor(c => c.CodeExpiration)
            .Must(date => date is null || date.Value.ToUniversalTime() > DateTime.UtcNow)
            .WithMessage("Expiration date must be in the future if provided.");
    }
}
