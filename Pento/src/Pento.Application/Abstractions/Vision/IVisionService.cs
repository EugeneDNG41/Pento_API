using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Azure.AI.Vision.ImageAnalysis;

namespace Pento.Application.Abstractions.Vision;
public interface IVisionService
{
    Task<ImageAnalysisResult> DetectObjectsAsync(Uri imageUrl);
}
