namespace HotelListing.Api.Data.Models
{
    public class QueryResult<TItem>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int RecordNumber { get; set; }
        public IEnumerable<TItem> Items { get; set; }
    }
}