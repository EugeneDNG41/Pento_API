using FluentValidation;
using Pento.Domain.Trades.Reports;

namespace Pento.Application.Trades.Reports.Create;

internal sealed class CreateTradeReportCommandValidator
    : AbstractValidator<CreateTradeReportCommand>
{
    public CreateTradeReportCommandValidator()
    {
        RuleFor(x => x.TradeSessionId)
            .NotEmpty();

        RuleFor(x => x.ReportedUserId)
            .NotEmpty();

        RuleFor(x => x)
            .Must(x => x.ReportedUserId != Guid.Empty)
            .WithMessage("Reported user is required.");

        RuleFor(x => x.Description)
            .MaximumLength(1000);
    }
}
