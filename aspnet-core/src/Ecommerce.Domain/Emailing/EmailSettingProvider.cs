using Volo.Abp.Settings;

namespace Ecommerce.Emailing;

public class EmailSettingProvider(ISettingEncryptionService encryptionService) : SettingDefinitionProvider
{
    public override void Define(ISettingDefinitionContext context)
    {
        var passwordSetting = context.GetOrNull("Abp.Mailing.Smtp.Password");
        if (passwordSetting is not null)
        {
            var debug =
                encryptionService.Encrypt(passwordSetting, "3286cba00748b58b5cc6daa14e6a49b2-0996409b-b1cf13ca");
        }
    }
}
