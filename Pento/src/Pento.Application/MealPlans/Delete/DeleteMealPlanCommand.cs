using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.MealPlans.Delete;

public sealed record DeleteMealPlanCommand(Guid Id) : ICommand;
