namespace VehicleManagement.Domain.Common.Wrappers;

public class PaginatedResponse<T> : Response<IEnumerable<T>>
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
    public long TotalRecords { get; set; }
    public long TotalCountRecords { get; set; }

    public PaginatedResponse(IEnumerable<T> data, int pageNumber, int pageSize, long totalRecords, long totalCount)
        : base(data)
    {
        PageNumber = pageNumber;
        PageSize = pageSize;
        TotalRecords = totalRecords;
        TotalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);
        TotalCountRecords = totalCount;
    }

    public PaginatedResponse(int statusCode, string message, bool success) : base(statusCode, message, success)
    {
    }
}

