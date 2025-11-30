using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Activities.Update;

public sealed record UpdateActivityCommand(
    string Code,
    string? Name,
    string? Description
) : ICommand;
