using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Messaging;
using Pento.Application.Abstractions.Utility.Pagination;
using Pento.Application.MealPlans.Get;
using Pento.Application.Users.GetUserRoles;
using Pento.Domain.MealPlans;
using Pento.Domain.Storages;

namespace Pento.Application.Storages.GetAll;
public sealed record GetStoragesQuery(string? SearchText, StorageType? StorageType, int PageNumber, int PageSize) : IQuery<PagedList<StoragePreview>>;
