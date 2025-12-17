using FluentValidation;
using Pento.Application.Abstractions.Utility.Pagination;

namespace Pento.Application.Notifications.Get;

internal sealed class GetNotificationsQueryValidator
    : AbstractValidator<GetNotificationsQuery>
{
    public GetNotificationsQueryValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0);

        RuleFor(x => x.SortOrder)
            .IsInEnum()
            .When(x => x.SortOrder.HasValue);
    }
}
