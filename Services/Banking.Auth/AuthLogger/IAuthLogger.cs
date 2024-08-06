using Banking.Auth.Models;

namespace Banking.Auth.Logger
{
    public interface IAuthLogger
    {
        void LogRequest(object dto, HttpContext httpContext);
        void LogResponse<T>(ResponseResult<T> response);
    }
}
