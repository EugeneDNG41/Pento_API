using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Compartments.GetAll;

public sealed record GetCompartmentsQuery(Guid StorageId) : IQuery<IReadOnlyList<CompartmentResponse>>;
