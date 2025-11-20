using Azure.AI.Vision.ImageAnalysis;

namespace Pento.Application.Vision;

public static class VisionMapper
{
    public static VisionAnalysisResponse ToResponse(ImageAnalysisResult result)
    {
        var response = new VisionAnalysisResponse
        {
            Caption = result.Caption?.Text
        };

        if (result.Tags != null)
        {
            response.Tags = result.Tags.Values.Select(t => t.Name).ToList();
        }
        if (result.Objects is not null && result.Objects.Values is not null)
        {
            response.Objects = result.Objects.Values.Select(o =>
            {
                string objectName = o.Tags.Count > 0 ? o.Tags[0].Name : "unknown";

                double confidence = 0;
                if (o.Tags.Count > 0)
                {
                    confidence = o.Tags[0].Confidence;
                    for (int i = 1; i < o.Tags.Count; i++)
                    {
                        if (o.Tags[i].Confidence > confidence)
                        {
                            confidence = o.Tags[i].Confidence;
                        }
                    }
                }

                return new VisionObjectDto
                {
                    Name = objectName,
                    Confidence = confidence,
                    BoundingBox = new VisionBoundingBoxDto
                    {
                        X = o.BoundingBox.X,
                        Y = o.BoundingBox.Y,
                        Width = o.BoundingBox.Width,
                        Height = o.BoundingBox.Height
                    }
                };
            }).ToList();
        }

        return response;
    }
}
