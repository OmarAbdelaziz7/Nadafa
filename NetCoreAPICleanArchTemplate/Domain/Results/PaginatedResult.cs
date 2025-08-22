namespace Domain.Results
{
    public class PaginatedResult<T>
    {
        public PaginatedResult(List<T> data = default!, int currentPage = 1, int totalCount = 0, int pageSize = 10, bool succeeded = true)
        {
            Data = data;
            CurrentPage = currentPage;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
            TotalCount = totalCount;
            PageSize = pageSize;
            Succeeded = succeeded;
        }

        public static PaginatedResult<T> Success(List<T> data, int count, int page, int pageSize)
        {
            return new(data, page, count, pageSize, true);
        }

        public List<T> Data { get; set; } = new List<T>();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalCount { get; set; }
        public int PageSize { get; set; }
        public bool Succeeded { get; set; }
        public string Message { get; set; } = string.Empty;
        public List<string>? Errors { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
