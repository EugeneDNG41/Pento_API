using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Get;
using Pento.Application.Storages.Get;
using Pento.Application.Users.GetPermissions;
using Pento.Domain.MealPlans;

namespace Pento.Application.Storages.GetAll;

public sealed record GetStorageByIdQuery(Guid StorageId) : IQuery<StorageResponse>;
