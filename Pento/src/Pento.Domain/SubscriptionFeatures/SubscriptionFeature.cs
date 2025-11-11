using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.Shared;

namespace Pento.Domain.SubscriptionFeatures;

public sealed class SubscriptionFeature : Entity
{
    public Guid SubscriptionId { get; private set; }
    public Guid FeatureId { get; private set; }
    public Limit? Limit { get; private set; }
}
public enum FeatureName
{
    AiOCR,
    RecipeSuggestions,
    FavoriteRecipes,


}
