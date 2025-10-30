using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Pento.Domain.Abstractions;

namespace Pento.Application.Abstractions.File;
public interface IBlobService
{
    Task<Result<string>> UploadFileAsync(IFormFile file, string fileTypeCategory = "general", CancellationToken cancellationToken = default);
    Task<Result<string>> UploadVideoAsync(IFormFile file, CancellationToken cancellationToken = default);

    Task<Result<string>> UploadImageAsync(IFormFile file, CancellationToken cancellationToken = default);
    Task<bool> DeleteFileAsync(string fileName, string fileTypeCategory = "general", CancellationToken cancellationToken = default);
    Task<bool> DeleteImageAsync(string fileName, CancellationToken cancellationToken = default);
    Task<bool> DeleteVideoAsync(string fileName, CancellationToken cancellationToken = default);

}
