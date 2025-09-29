using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;

namespace Pento.Modules.Community.Application.GiveawayPosts.Create;
internal sealed class CreateGiveawayPostCommandValidator : AbstractValidator<CreateGiveawayPostCommand>
{
    public CreateGiveawayPostCommandValidator()
    {
    }
}
