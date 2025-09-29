using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Common.Application.Messaging;

namespace Pento.Modules.Community.Application.Comments.Create;
public sealed record CreateCommentCommand(): ICommand<Guid>;

