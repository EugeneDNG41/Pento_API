using FluentValidation;

namespace Pento.Application.Notifications.DeviceTokens;

internal sealed class RegisterDeviceTokenCommandValidator
    : AbstractValidator<RegisterDeviceTokenCommand>
{
    public RegisterDeviceTokenCommandValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Device token is required.")
            .MinimumLength(10).WithMessage("Device token is too short.")
            .MaximumLength(500).WithMessage("Device token is too long.");

        RuleFor(x => x.Platform)
            .IsInEnum().WithMessage("Invalid device platform.");
    }
}
