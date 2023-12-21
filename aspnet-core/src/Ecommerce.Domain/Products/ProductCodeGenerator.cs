using System.Threading.Tasks;
using Ecommerce.IdentitySettings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Products;

public class ProductCodeGenerator(IRepository<IdentitySetting, string> identitySettingRepository)
    : ITransientDependency
{
    public async Task<string> GenerateAsync()
    {
        string newCode;
        var identitySetting = await identitySettingRepository.FindAsync(EcommerceConsts.ProductIdentitySettingId);
        if (identitySetting is null)
        {
            identitySetting = await identitySettingRepository.InsertAsync(new IdentitySetting(
                EcommerceConsts.ProductIdentitySettingId, "Sản phẩm", EcommerceConsts.ProductIdentitySettingPrefix, 1, 1));
            newCode = identitySetting.Prefix + identitySetting.CurrentNumber;
        }
        else
        {
            identitySetting.CurrentNumber += identitySetting.StepNumber;
            newCode = identitySetting.Prefix + identitySetting.CurrentNumber;

            await identitySettingRepository.UpdateAsync(identitySetting);
        }
        return newCode;
    }
} 
