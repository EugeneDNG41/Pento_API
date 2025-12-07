using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Get;

public sealed record GetDietaryTagByIdQuery(Guid Id) : IQuery<DietaryTagResponse>;
