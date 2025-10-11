using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Domain.FoodReferences;

public interface IFoodReferenceRepository
{
    Task<FoodReference?> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<FoodReference?> GetByBarcodeAsync(string barcode, CancellationToken cancellationToken = default);
    Task AddAsync(FoodReference foodReference, CancellationToken cancellationToken = default);
}
