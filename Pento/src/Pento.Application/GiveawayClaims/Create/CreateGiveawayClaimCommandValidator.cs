using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Application.GiveawayClaims.Create;
internal sealed class CreateGiveawayClaimCommandValidator : AbstractValidator<CreateGiveawayClaimCommand>

{ 
    public CreateGiveawayClaimCommandValidator()
    {
    }
}
