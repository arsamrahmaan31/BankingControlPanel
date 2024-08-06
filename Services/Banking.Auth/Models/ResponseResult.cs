namespace Banking.Auth.Models
{
    public class ResponseResult<T>
    {
        public bool success { get; set; }
        public T? result { get; set; }
        public string? message { get; set; }
        public string? token { get; set; }
    }
}
