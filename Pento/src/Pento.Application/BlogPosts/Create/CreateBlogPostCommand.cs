using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.BlogPosts.Create;
public sealed record CreateBlogPostCommand(
) : ICommand<Guid>;
