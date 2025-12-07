using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Activities.GetAll;

public sealed record GetActivitiesQuery(string? SearchText) : IQuery<IReadOnlyList<ActivityResponse>>;
