using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.FoodItems.GetById;
using Pento.Application.FoodItems.Projections;

namespace Pento.Application.FoodItems.GetMergeCandidates;

public sealed record GetFoodItemMergeCandidatesQuery(Guid Id) : IQuery<IReadOnlyList<FoodItemPreview>>;

