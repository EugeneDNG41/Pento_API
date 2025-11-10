using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Marten;
using Pento.Application.Abstractions.Clock;
using Quartz;

namespace Pento.Infrastructure.Quartz;
#pragma warning disable CS1998 
#pragma warning disable CS9113

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob(IDateTimeProvider dateTimeProvider, IDocumentSession session) : IJob // + subscription notification job
{
    public async Task Execute(IJobExecutionContext context)
    {
    }
}
