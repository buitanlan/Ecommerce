using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using Volo.Abp.EventBus;

namespace Ecommerce.Orders.Events;

public class ChangeStockCountEventHandler:  ILocalEventHandler<NewOrderCreatedEvent>,
                                                    ITransientDependency
{
    public Task HandleEventAsync(NewOrderCreatedEvent eventData)
    {
        throw new System.NotImplementedException();
    }
}
