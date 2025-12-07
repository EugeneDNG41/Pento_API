using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Recipes.Delete;

public sealed record DeleteRecipeCommand(Guid Id) : ICommand;
