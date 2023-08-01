using FluentValidation;

namespace Ecommerce.Admin.Products;

public class AddUpdateProductAttributeDtoValidator : AbstractValidator<AddUpdateProductAttributeDto>
{
    public AddUpdateProductAttributeDtoValidator()
    {
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.AttributeId).NotEmpty();
    }
}
