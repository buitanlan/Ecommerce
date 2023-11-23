using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Guids;
using Volo.Abp.Identity;
using Volo.Abp.MultiTenancy;
using Volo.Abp.Uow;

namespace Ecommerce.Seeding;

public class IdentityDataSeeder(
    IGuidGenerator guidGenerator,
    IIdentityRoleRepository roleRepository,
    IIdentityUserRepository userRepository,
    ILookupNormalizer lookupNormalizer,
    IdentityUserManager userManager,
    IdentityRoleManager roleManager,
    ICurrentTenant currentTenant,
    IOptions<IdentityOptions> identityOptions)
    : ITransientDependency, IIdentityDataSeeder
{
    [UnitOfWork]
    public virtual async Task<IdentityDataSeedResult> SeedAsync(string adminEmail, string adminPassword, Guid? tenantId = null)
    {
        using (currentTenant.Change(tenantId))
        {
            await identityOptions.SetAsync();

            var result = new IdentityDataSeedResult();
            //"admin" user
            var adminUser = await userRepository.FindByNormalizedUserNameAsync(
                lookupNormalizer.NormalizeName(adminEmail)
            );

            if (adminUser != null)
            {
                return result;
            }

            adminUser = new IdentityUser(
                guidGenerator.Create(),
                adminEmail,
                adminEmail,
                tenantId
            )
            {
                Name = "Admin"
            };

            (await userManager.CreateAsync(adminUser, adminPassword, validatePassword: false)).CheckErrors();
            result.CreatedAdminUser = true;

            //"admin" role
            const string adminRoleName = "Admin";
            var adminRole =
                await roleRepository.FindByNormalizedNameAsync(lookupNormalizer.NormalizeName(adminRoleName));
            if (adminRole is null)
            {
                adminRole = new IdentityRole(
                    guidGenerator.Create(),
                    adminRoleName,
                    tenantId
                )
                {
                    IsStatic = true,
                    IsPublic = true
                };

                (await roleManager.CreateAsync(adminRole)).CheckErrors();
                result.CreatedAdminRole = true;
            }

            (await userManager.AddToRoleAsync(adminUser, adminRoleName)).CheckErrors();

            return result;
        }
    }
}
