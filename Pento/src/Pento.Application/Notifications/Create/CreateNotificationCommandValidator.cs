using FluentValidation;
using Pento.Domain.Notifications;

namespace Pento.Application.Notifications.Create;

public sealed class CreateNotificationCommandValidator
    : AbstractValidator<CreateNotificationCommand>
{
    public CreateNotificationCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Notification title is required.")
            .MaximumLength(200).WithMessage("Title is too long.");

        RuleFor(x => x.Body)
            .NotEmpty().WithMessage("Notification body is required.")
            .MaximumLength(500).WithMessage("Body is too long.");

        RuleFor(x => x.Type)
            .IsInEnum().WithMessage("Invalid notification type.");

        RuleFor(x => x.Payload)
            .NotNull().WithMessage("Payload cannot be null.");
    }

}
