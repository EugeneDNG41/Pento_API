using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Pento.Infrastructure.Quartz;
#pragma warning disable CS1998 

[DisallowConcurrentExecution]
internal sealed class ProcessExpirationDateTrackingJob : IJob // + reset feature usage and point earned job + subscription notification job
{
    public async Task Execute(IJobExecutionContext context)
    {
    }
}
