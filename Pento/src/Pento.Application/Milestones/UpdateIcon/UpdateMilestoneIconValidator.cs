using FluentValidation;

namespace Pento.Application.Milestones.UpdateIcon;

internal sealed class UpdateMilestoneIconValidator : AbstractValidator<UpdateMilestoneIconCommand>
{
    public UpdateMilestoneIconValidator()
    {
        RuleFor(x => x.MilestoneId)
            .NotEmpty().WithMessage("Milestone Id is required.");
        RuleFor(x => x.IconFile)
            .NotNull().WithMessage("Icon file is required.")
            .Must(file => file.Length > 0).WithMessage("Icon file cannot be empty.")
            .Must(file => file.ContentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)).WithMessage("Icon file must be an image.");
    }
}

