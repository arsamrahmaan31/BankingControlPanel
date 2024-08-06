namespace Banking.Client.ClientLogger
{
    public interface IClientLogger
    {
        void LogRequest(object dto, HttpContext httpContext);
        void LogResponse<T>(ResponseResult<T> response);
    }
}
