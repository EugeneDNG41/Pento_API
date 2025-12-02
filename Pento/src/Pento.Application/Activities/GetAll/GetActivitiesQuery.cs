using System.Diagnostics;
using Pento.Application.Abstractions.Messaging;
using Pento.Domain.Activities;

namespace Pento.Application.Activities.GetAll;

public sealed record GetActivitiesQuery(string? SearchText) : IQuery<IReadOnlyList<ActivityResponse>>;
