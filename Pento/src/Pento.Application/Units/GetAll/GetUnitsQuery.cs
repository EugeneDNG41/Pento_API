using Pento.Application.Abstractions.Messaging;
using Pento.Application.Units.Get;

namespace Pento.Application.Units.GetAll;

public sealed record GetUnitsQuery() : IQuery<IReadOnlyList<UnitResponse>>;
