namespace JaCore.Api.DTOs.Common
{
    public class QueryParametersDto
    {
        private const int MaxPageSize = 50;
        private int _pageSize = 10;
        public int PageNumber { get; set; } = 1;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }

        public string? SortBy { get; set; } // e.g., "Name asc", "ModifiedAt desc"
        public string? SearchQuery { get; set; } // General search term
        public string? Include { get; set; } // Comma-separated string of navigation properties to include
    }
} 