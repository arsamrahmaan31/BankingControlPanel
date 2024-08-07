namespace Banking.Client.Models
{
    public class QueryParameters
    {
        public string? filter { get; set; }
        public string? sortBy { get; set; }
        public bool sortDescending { get; set; } = false;
        public int pageNumber { get; set; } = 1;
        public int pageSize { get; set; } = 10;
    }
}
