﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.DependencyInjection;
using Pento.Application.Abstractions.Authorization;
using Pento.Application.Abstractions.Exceptions;
using Pento.Domain.Abstractions;
using Pento.Infrastructure.Authentication;

namespace Pento.Infrastructure.Authorization;

internal sealed class CustomClaimsTransformation(IServiceScopeFactory serviceScopeFactory) : IClaimsTransformation
{
    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        using IServiceScope scope = serviceScopeFactory.CreateScope();

        IPermissionService permissionService = scope.ServiceProvider.GetRequiredService<IPermissionService>();

        string identityId = principal.GetIdentityId();

        Result<PermissionsResponse> result = await permissionService.GetUserPermissionsAsync(identityId);

        if (result.IsFailure)
        {
            throw new PentoException(nameof(IPermissionService.GetUserPermissionsAsync), result.Error);
        }

        var claimsIdentity = new ClaimsIdentity();

        claimsIdentity.AddClaim(new Claim(CustomClaims.User, result.Value.UserId.ToString()));
        if (result.Value.HouseholdId is not null)
        {
            claimsIdentity.AddClaim(new Claim(CustomClaims.Household, result.Value.HouseholdId.ToString()!));
        }
        
        foreach (string roles in result.Value.Roles)
        {
            claimsIdentity.AddClaim(new Claim(ClaimTypes.Role, roles));
        }

        principal.AddIdentity(claimsIdentity);

        return principal;
    }
}
