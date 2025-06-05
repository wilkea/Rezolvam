namespace rezolvam.Application.Common
{
    public class PagedList<T>
    {
        public List<T> Items { get; private set; }
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }

        public bool HasPrevious => PageIndex > 1;
        public bool HasNext => PageIndex < TotalPages;
        
        public PagedList(List<T> items, int pageIndex, int totalCount, int pageSize)
        {
            Items = items;
            PageIndex = pageIndex;
            TotalCount = totalCount;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        }
    }
}