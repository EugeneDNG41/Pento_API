using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.File;
public interface IUnsplashImageService
{
    Task<Result<Uri>> GetImageUrlAsync(string query, CancellationToken cancellationToken = default);
}
