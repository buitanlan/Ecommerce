using System.Threading.Tasks;
using Ecommerce.IdentitySettings;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Domain.Repositories;

namespace Ecommerce.Orders;

public class OrderCodeGenerator(IRepository<IdentitySetting, string> identitySettingRepository)
    : ITransientDependency
{
    public async Task<string> GenerateAsync()
    {
        string newCode;
        var identitySetting = await identitySettingRepository.FindAsync(EcommerceConsts.OrderIdentitySettingId);
        if (identitySetting == null)
        {
            identitySetting = await identitySettingRepository.InsertAsync(new IdentitySetting(
                EcommerceConsts.OrderIdentitySettingId, "Sản phẩm", EcommerceConsts.ProductIdentitySettingPrefix, 1,
                1));
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
