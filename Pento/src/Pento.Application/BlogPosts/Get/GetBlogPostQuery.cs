using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.BlogPosts.Get;
public sealed record GetBlogPostQuery(Guid PostId) : IQuery<BlogPostResponse>;
