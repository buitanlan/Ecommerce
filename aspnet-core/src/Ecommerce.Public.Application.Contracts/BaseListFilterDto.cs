using Volo.Abp.Application.Dtos;

namespace Ecommerce;

public class BaseListFilterDto : PagedResultRequestDto
{
    public string Keyword { get; set; }
}
