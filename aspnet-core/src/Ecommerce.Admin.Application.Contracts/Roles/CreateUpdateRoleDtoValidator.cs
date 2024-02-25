using FluentValidation;

namespace Ecommerce.Admin.Roles;

public class CreateUpdateRoleDtoValidator : AbstractValidator<CreateUpdateRoleDto>
{
    public CreateUpdateRoleDtoValidator()
    {
        RuleFor(x => x.Name).NotEmpty();
        RuleFor(x => x.Description).NotEmpty();
    }
}
