using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StarKindred.API.Entities;
using StarKindred.API.Exceptions;

namespace StarKindred.API.Middleware;

public class AppExceptionFilter: IExceptionFilter
{
    private IWebHostEnvironment Environment { get; }

    public AppExceptionFilter(IWebHostEnvironment environment)
    {
        Environment = environment;
    }

    public void OnException(ExceptionContext context)
    {
        if (context.Exception is AppException appException)
            HandleAppException(context, appException);
        else if(!Environment.IsDevelopment())
            HandleOtherException(context);
    }

    private static void HandleAppException(ExceptionContext context, AppException appException)
    {
        context.HttpContext.Response.StatusCode = appException.StatusCode;

        if(appException is TooFastException tooFast)
            context.HttpContext.Response.Headers.Add("Retry-After", tooFast.RetryInSeconds.ToString());
        
        context.Result = new JsonResult(new ApiResponse()
        {
            Messages = new() { ApiMessage.Error(appException.Message) }
        });

        context.ExceptionHandled = true;
    }

    private static void HandleOtherException(ExceptionContext context)
    {
        context.HttpContext.Response.StatusCode = 500;

        context.Result = new JsonResult(new ApiResponse()
        {
            Messages = new()
            {
                ApiMessage.Error("A ~mysterious~ error occurred. Ben (the developer) has been notified. Feel free to try again, but don't be too alarmed if the problem continues. (Sorry about that!)")
            }
        });

        context.ExceptionHandled = true;
    }
}