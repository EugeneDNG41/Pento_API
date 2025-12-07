using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Delete;

public sealed record DeleteDietaryTagCommand(Guid Id) : ICommand<Guid>;
