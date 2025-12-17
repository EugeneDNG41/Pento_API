using FluentValidation;
using Microsoft.AspNetCore.Http;
using Pento.Domain.Trades;

namespace Pento.Application.Trades.Reports.CreateReportMedias;

internal sealed class AddTradeReportMediaCommandValidator
    : AbstractValidator<AddTradeReportMediaCommand>
{
    public AddTradeReportMediaCommandValidator()
    {
        RuleFor(x => x.TradeReportId)
            .NotEmpty();

        RuleFor(x => x.File)
            .NotNull()
            .Must(f => f.Length > 0)
            .WithMessage("File is required.");
    }
}
