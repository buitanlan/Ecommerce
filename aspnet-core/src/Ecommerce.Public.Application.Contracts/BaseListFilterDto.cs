using Ecommerce.Public.Application.Contracts;

namespace Ecommerce;

public class BaseListFilterDto : PagedResultRequestBase
{
    public string Keyword { get; set; }
}
