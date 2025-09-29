using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;
using Pento.Common.Domain;

namespace Pento.Modules.Community.Application.Comments.Get;
internal sealed class GetCommentQueryHandler() : IQueryHandler<GetCommentQuery, CommentResponse>
{
    public Task<Result<CommentResponse>> Handle(GetCommentQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
