using System.ComponentModel.DataAnnotations;

namespace Pento.Infrastructure.External.PayOS;

public sealed class PayOSCustomOptions
{
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ApiKey { get; set; }
    [Required]
    public string ChecksumKey { get; set; }
    [Required]
    public string WebhookUrl { get; set; }
    [Required]
    public string ReturnUrl { get; set; }
    [Required]
    public string CancelUrl { get; set; }

}
