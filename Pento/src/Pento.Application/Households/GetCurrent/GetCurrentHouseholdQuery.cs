
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GetCurrent;

public sealed record GetCurrentHouseholdQuery() : IQuery<HouseholdResponse>;

