using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.MealPlans.Get;
using Pento.Application.Storages.GetById;
using Pento.Application.Users.GetUserRoles;
using Pento.Domain.MealPlans;

namespace Pento.Application.Storages.GetAll;
public sealed record GetStoragesQuery() : IQuery<IReadOnlyList<StorageResponse>>;
