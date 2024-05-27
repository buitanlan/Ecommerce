using System.Threading.Tasks;
using Ecommerce.Emailing;
using Volo.Abp.DependencyInjection;
using Volo.Abp.Emailing;
using Volo.Abp.EventBus;
using Volo.Abp.TextTemplating;

namespace Ecommerce.Orders.Events;

public class SendMailtoCustomerEventHandler(
   IEmailSender emailSender,
   ITemplateRenderer templateRenderer)
   : ILocalEventHandler<NewOrderCreatedEvent>,
      ITransientDependency
{
   public async Task HandleEventAsync(NewOrderCreatedEvent eventData)
   {
      var emailBody = await templateRenderer.RenderAsync(
                 EmailTemplates.CreateOrderEmail,
                 new
                 {
                     message = eventData.Message
                 });
      await emailSender.SendAsync(eventData.CustomerEmail, "Tạo đơn hàng thành công", emailBody);
   }
}
