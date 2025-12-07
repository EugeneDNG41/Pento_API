using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Units.Get;

public sealed record GetUnitQuery(Guid UnitId) : IQuery<UnitResponse>;

