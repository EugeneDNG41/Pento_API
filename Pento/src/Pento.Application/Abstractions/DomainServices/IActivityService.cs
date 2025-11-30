using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;
using Pento.Domain.UserActivities;

namespace Pento.Application.Abstractions.DomainServices;

public interface IActivityService
{
    Task<Result<UserActivity>> RecordActivityAsync(Guid userId, string activityCode, CancellationToken cancellationToken);
}
