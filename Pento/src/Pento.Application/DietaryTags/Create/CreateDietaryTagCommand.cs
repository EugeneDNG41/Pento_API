using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.DietaryTags.Create;

public sealed record CreateDietaryTagCommand(
    string Name,
    string? Description
) : ICommand<Guid>;
