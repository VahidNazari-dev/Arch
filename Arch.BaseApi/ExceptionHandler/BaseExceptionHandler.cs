using Microsoft.AspNetCore.Mvc.Filters;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting;

namespace Arch.BaseApi;

public class BaseExceptionHandler : IExceptionFilter, IFilterMetadata
{
    private readonly Microsoft.Extensions.Hosting.IHostingEnvironment _environment;

    private readonly ILogger<BaseExceptionHandler> _logger;

    public BaseExceptionHandler(Microsoft.Extensions.Hosting.IHostingEnvironment environment, ILogger<BaseExceptionHandler> logger)
    {
        _environment = environment;
        _logger = logger;
    }
    public void OnException(ExceptionContext context)
    {
        Exception exception = context.Exception;
        ApiResult result;
        ValidationException validationExcept = exception as ValidationException;
        UnauthorizedAccessException unAuthExcept = exception as UnauthorizedAccessException;
        if (validationExcept != null)
        {
            if (!string.IsNullOrEmpty(validationExcept.SecondResponseMessageKey))
                result = new ApiResult( HttpStatusCode.BadRequest, validationExcept.ResponseMessageKey, validationExcept.SecondResponseMessageKey ?? validationExcept.ResponseMessageKey, validationExcept.Errors);
            else
                result = new ApiResult( HttpStatusCode.BadRequest, validationExcept.ResponseMessageKey, validationExcept.Errors);
            context.ExceptionHandled = true;
        }
        else if (unAuthExcept != null)
        {
            result = new ApiResult( HttpStatusCode.Unauthorized, "AuthenticationFailed", new ResponseError[] { new ResponseError("authentication", "AuthenticationFailed") });
            context.ExceptionHandled = true;
        }
        else
        {
            _logger.LogError(exception.ToString());
            string text = "Internal Error, Please contact support";
            string text2 = ((!_environment.IsProduction()) ? exception.ToString() : text);
            result = new ApiResult( HttpStatusCode.InternalServerError, "ExceptionThrow", new ResponseError[] { new ResponseError("system", text2) });
            context.ExceptionHandled = true;
        }
        context.Result = result;
        return;
    }
}
