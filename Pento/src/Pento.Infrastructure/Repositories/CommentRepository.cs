using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Comments;
using Pento.Domain.GiveawayClaims;

namespace Pento.Infrastructure.Repositories;

internal sealed class CommentRepository(ApplicationDbContext context) : Repository<GiveawayClaim>(context), ICommentRepository
{
}
