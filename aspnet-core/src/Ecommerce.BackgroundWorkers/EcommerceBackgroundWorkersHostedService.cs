using Microsoft.Extensions.Hosting;
using Volo.Abp;

namespace TeduEcommerce.BackgroundWorkers;

public class EcommerceBackgroundWorkersHostedService(
    IAbpApplicationWithExternalServiceProvider application,
    IServiceProvider serviceProvider)
    : IHostedService
{
    public Task StartAsync(CancellationToken cancellationToken)
    {
        application.Initialize(serviceProvider);

        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        application.Shutdown();

        return Task.CompletedTask;
    }
}
