using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using StarKindred.API.Entities;

namespace StarKindred.API.Middleware;

public class InvalidModelStateFilter: IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.ModelState.IsValid) return;
        
        context.HttpContext.Response.StatusCode = 422;
            
        var errors = context.ModelState.Values
            .Where(v => v.Errors.Count > 0)
            .Select(v => ApiMessage.Error(v.Errors.Last().ErrorMessage))
            .ToList()
        ;
            
        context.Result = new JsonResult(new ApiResponse()
        {
            Messages = errors
        });
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}