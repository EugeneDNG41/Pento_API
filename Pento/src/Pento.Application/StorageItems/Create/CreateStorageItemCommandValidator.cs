using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.StorageItems.Create;

internal sealed class CreateStorageItemCommandValidator : AbstractValidator<CreateStorageItemCommand>
{
    public CreateStorageItemCommandValidator()
    {     
    }
}
