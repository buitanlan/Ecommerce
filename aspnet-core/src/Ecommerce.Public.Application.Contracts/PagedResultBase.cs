using System;

namespace Ecommerce.Public.Application.Contracts;

public abstract class PagedResultBase
{
    public long CurrentPage { get; set; }

    public long PageCount
    {
        get
        {
            var pageCount = (double)RowCount / PageSize;
            return (int)Math.Ceiling(pageCount);
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            PageCount = value;
        }

    }

    public long RowCount { get; set; }
    public long PageSize { get; set; }
    public long FirstRowOnPage => (CurrentPage - 1) * PageSize + 1;
    public long LastRowOnPage => Math.Min(CurrentPage * PageSize, RowCount);
    public string AdditionData { get; set; }
}
