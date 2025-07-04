using AutoMapper;

namespace Application.Common.DTO;

public class PaginationDTO<TOut>
{
    public required List<TOut> Data { get; set; }
    public int CurrentPage { get; set; }
    public int PerPage { get; set; }
    public int TotalItems { get; set; }
    public int TotalPages { get; set; }

    public static PaginationDTO<TOut> CreatePagination(IQueryable<TOut> data, int page, int limit)
    {
        var offset = (page - 1) * limit;
        var totalItems = data.Count();
        var totalPages = totalItems / limit;

        var pagination = new PaginationDTO<TOut>
        {
            Data = data.Skip(offset).Take(limit).ToList(),
            CurrentPage = page,
            PerPage = limit,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };

        return pagination;
    }
    public static PaginationDTO<TOut> CreateMappedPagination<TIn>(IQueryable<TIn> data, int page, int limit, IMapper mapper)
    {
        var offset = (page - 1) * limit;
        var totalItems = data.Count();
        var totalPages = (int)Math.Ceiling((double)totalItems / limit);

        var pagination = new PaginationDTO<TOut>
        {
            Data = mapper.Map<List<TOut>>(data.Skip(offset).Take(limit)),
            CurrentPage = page,
            PerPage = limit,
            TotalItems = totalItems,
            TotalPages = totalPages,
        };

        return pagination;
    }
}
