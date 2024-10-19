using FluentValidation;

namespace Ecommerce.Admin.ProductAttributes;

public sealed class CreateUpdateProductAttributeDtoValidator : AbstractValidator<CreateUpdateProductAttributeDto>
{
    public CreateUpdateProductAttributeDtoValidator()
    {
        RuleFor(x => x.Label).NotEmpty().MaximumLength(50);
        RuleFor(x => x.Code).NotEmpty().MaximumLength(50);
        RuleFor(x => x.DataType).NotNull();
    }
}
