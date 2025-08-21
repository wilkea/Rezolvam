using rezolvam.Domain.Reports.StatusChanges.Enums;

namespace rezolvam.Application.DTOs.Common
{
    public class PaginationRequest
    {
        private int _pageIndex = 1;
        private int _pageSize = 9;

        public int PageIndex
        {
            get => _pageIndex;
            set => _pageIndex = value;
        }

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value;
        }

        // WHY: Ensures PageIndex is never less than 1 to prevent negative OFFSET
        // HOW: Math.Max forces minimum value of 1, even if input is 0 or negative
        public int ValidatedPageIndex => Math.Max(1, PageIndex);

        // WHY: Prevents excessive page sizes that could impact performance
        // HOW: Clamps between reasonable bounds (1-100)
        public int ValidatedPageSize => Math.Max(1, Math.Min(100, PageSize));

    }
}