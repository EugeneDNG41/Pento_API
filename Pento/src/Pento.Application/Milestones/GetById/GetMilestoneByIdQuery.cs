using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Messaging;

namespace Pento.Application.Milestones.GetById;

public sealed record GetMilestoneByIdQuery(Guid Id) : IQuery<MilestoneDetailResponse>;
