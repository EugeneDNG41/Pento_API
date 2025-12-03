using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Households.GetById;

public sealed record GetHouseholdByIdQuery(Guid HouseholdId) : IQuery<HouseholdAdminResponse>;
