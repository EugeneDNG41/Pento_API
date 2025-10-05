using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Pento.Application.Abstractions.Authentication;
using Pento.Domain.StorageItems;

namespace Pento.Modules.Pantry.Infrastructure.FoodItems;

internal sealed class FoodItemConfiguration : IEntityTypeConfiguration<StorageItem>
{
    public void Configure(EntityTypeBuilder<StorageItem> builder)
    {
        
    }
}
