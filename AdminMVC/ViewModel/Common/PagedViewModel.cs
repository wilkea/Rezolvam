namespace AdminMVC.ViewModel.Common
{
    public class PagedViewModel<T>
    {
        public IReadOnlyList<T> Items { get; set; } = new List<T>();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasPreviousPage { get; set; }
        public bool HasNextPage { get; set; }

        // WHY: UI-specific properties that don't belong in DTOs
        // HOW: Calculate display values for better UX
        public int DisplayPageNumber => PageIndex + 1; // Users expect 1-based page numbers
        public int StartItem => PageIndex * PageSize + 1;
        public int EndItem => Math.Min((PageIndex + 1) * PageSize, TotalCount);
        public string PaginationText => TotalCount == 0 ? "No items" : $"Showing {StartItem}-{EndItem} of {TotalCount}";

        // WHY: Helper for generating page navigation links
        // HOW: Provides page numbers for pagination controls
        public IEnumerable<int> GetPageNumbers(int maxPages = 5)
        {
            var start = Math.Max(0, PageIndex - maxPages / 2);
            var end = Math.Min(TotalPages - 1, start + maxPages - 1);
            start = Math.Max(0, end - maxPages + 1);

            return Enumerable.Range(start, end - start + 1);
        }
    }
}