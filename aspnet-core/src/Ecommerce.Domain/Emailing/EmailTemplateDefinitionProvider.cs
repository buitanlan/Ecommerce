using Volo.Abp.TextTemplating;
using Volo.Abp.TextTemplating.Scriban;

namespace Ecommerce.Emailing;

public class EmailTemplateDefinitionProvider : TemplateDefinitionProvider
{
    public override void Define(ITemplateDefinitionContext context)
    {
        context.Add(
            new TemplateDefinition(
                    EmailTemplates.Layout,
                    isLayout: true //SET isLayout!
                )
                .WithScribanEngine()
                .WithVirtualFilePath(
                    "/Emailing/Templates/Layout.tpl",
                    isInlineLocalized: true
                ));

        context.Add(
            new TemplateDefinition(
                    name: EmailTemplates.WelcomeEmail,
                    layout: EmailTemplates.Layout
                )
                .WithVirtualFilePath(
                    "/Emailing/Templates/WelcomeEmail.tpl",
                    isInlineLocalized: true
                ).WithScribanEngine()
        );
        context.Add(
            new TemplateDefinition(
                    name: EmailTemplates.CreateOrderEmail,
                    layout: EmailTemplates.Layout
                )
                .WithVirtualFilePath(
                    "/Emailing/Templates/CreateOrderEmail.tpl",
                    isInlineLocalized: true
                ).WithScribanEngine()
        );
    }
}
