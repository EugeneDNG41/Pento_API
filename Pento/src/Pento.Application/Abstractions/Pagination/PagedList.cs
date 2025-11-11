using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pento.Application.Abstractions.Pagination;

public sealed class PagedList<T> : List<T>
{
    public Metadata Metadata { get; init; }
    public PagedList(List<T> items, int count, int pageNumber, int pageSize)
    {
        Metadata = new Metadata
        {
            CurrentPage = pageNumber,
            TotalPages = (int)Math.Ceiling(count / (double)pageSize),
            PageSize = pageSize,
            TotalCount = count
        };
        AddRange(items);
    }
    public static PagedList<T> Create(IReadOnlyList<T> source, int count, int pageNumber, int pageSize)
    {
        return new PagedList<T>(source.ToList(), count, pageNumber, pageSize);
    }
}
public sealed class Metadata
{
    public int CurrentPage { get; init; }
    public int TotalPages { get; init; }
    public int PageSize { get; init; }
    public int TotalCount { get; init; }
    public bool HasPrevious => CurrentPage > 1;
    public bool HasNext => CurrentPage < TotalPages;
}
