using System.Collections.Generic;
using System.Security.Claims;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Security.Claims;

namespace Ecommerce.Security;

[Dependency(ReplaceServices = true)]
public class FakeCurrentPrincipalAccessor : ThreadCurrentPrincipalAccessor
{
    protected override ClaimsPrincipal GetClaimsPrincipal()
    {
        return GetPrincipal();
    }

    private ClaimsPrincipal _principal;

    private ClaimsPrincipal GetPrincipal()
    {
        if (_principal != null)
        {
            return _principal;
        }

        lock (this)
        {
            _principal ??= new ClaimsPrincipal(
                new ClaimsIdentity(
                    new List<Claim>
                    {
                        new(AbpClaimTypes.UserId, "2e701e62-0953-4dd3-910b-dc6cc93ccb0d"),
                        new(AbpClaimTypes.UserName, "admin"),
                        new(AbpClaimTypes.Email, "admin@abp.io")
                    }
                )
            );
        }

        return _principal;
    }
}
