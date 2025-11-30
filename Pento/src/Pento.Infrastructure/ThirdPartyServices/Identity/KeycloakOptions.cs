using System.ComponentModel.DataAnnotations;

namespace Pento.Infrastructure.ThirdPartyServices.Identity;

public sealed class KeycloakOptions
{
    [Required]
    public string Authority { get; set; }
    [Required]
    public string AdminUrl { get; set; }
    [Required]
    public string TokenUrl { get; set; }
    [Required]
    public string ClientId { get; set; }
    [Required]
    public string ClientSecret { get; set; }
}
