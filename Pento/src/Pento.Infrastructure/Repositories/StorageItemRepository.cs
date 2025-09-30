using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.GiveawayClaims;
using Pento.Domain.StorageItems;
using Pento.Infrastructure;
using Pento.Infrastructure.Repositories;

namespace Pento.Modules.Pantry.Infrastructure.FoodItems;

internal sealed class StorageItemRepository(ApplicationDbContext context) : Repository<GiveawayClaim>(context), IStorageItemRepository
{
}
