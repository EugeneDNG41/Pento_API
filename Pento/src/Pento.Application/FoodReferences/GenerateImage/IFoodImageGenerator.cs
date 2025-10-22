using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.FoodReferences.GenerateImage;
public interface IFoodImageGenerator
{
    Task<string?> GenerateImageAsync(Guid foodId, CancellationToken ct);
}
