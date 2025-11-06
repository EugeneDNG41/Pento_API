using Pento.Application.Abstractions.Messaging;
using Pento.Application.Compartments.Get;

namespace Pento.Application.Compartments.GetAll;

public sealed record GetCompartmentsQuery(Guid StorageId) : IQuery<IReadOnlyList<CompartmentWithFoodItemPreviewResponse>>;
