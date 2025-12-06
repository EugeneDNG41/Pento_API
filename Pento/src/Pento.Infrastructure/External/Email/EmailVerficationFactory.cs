using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Pento.Infrastructure.External.Email;

internal sealed class EmailVerficationFactory(
    HttpContextAccessor httpContextAccessor,
    LinkGenerator linkGenerator)
{
    public string CreateEmailVerificationLink(Guid verificationId)
    {
        HttpContext httpContext = httpContextAccessor.HttpContext!;
        string? verificationPath = linkGenerator.GetUriByName(
            httpContext,
            endpointName: "VerifyEmail",
            values: new { VerificationId = verificationId });
        string verificationUrl = string.Concat(
            httpContext.Request.Scheme,
            "://",
            httpContext.Request.Host.ToUriComponent(),
            verificationPath);
        return verificationUrl;
    }
}
