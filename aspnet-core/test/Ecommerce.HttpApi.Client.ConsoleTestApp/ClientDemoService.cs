using System;
using System.Threading.Tasks;
using Volo.Abp.Account;
using Volo.Abp.DependencyInjection;

namespace Ecommerce.HttpApi.Client.ConsoleTestApp;

public class ClientDemoService(IProfileAppService profileAppService) : ITransientDependency
{
    public async Task RunAsync()
    {
        var output = await profileAppService.GetAsync();
        Console.WriteLine($"UserName : {output.UserName}");
        Console.WriteLine($"Email    : {output.Email}");
        Console.WriteLine($"Name     : {output.Name}");
        Console.WriteLine($"Surname  : {output.Surname}");
    }
}
