namespace Ecommerce.Public.Application.Contracts;

public class PagedResultRequestBase
{
    public int CurrentPage { get; set; }
    public int PageSize { get; set; }
}
