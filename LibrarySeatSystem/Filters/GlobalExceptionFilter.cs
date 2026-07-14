using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace LibrarySeatSystem.Filters;

public class GlobalExceptionFilter : IExceptionFilter
{
    private readonly ILogger<GlobalExceptionFilter> _logger;

    public GlobalExceptionFilter(ILogger<GlobalExceptionFilter> logger)
    {
        _logger = logger;
    }

    public void OnException(ExceptionContext context)
    {
        _logger.LogError(context.Exception, "未处理的异常: {Message}", context.Exception.Message);

        context.Result = new ViewResult
        {
            ViewName = "Error",
            ViewData = new Microsoft.AspNetCore.Mvc.ViewFeatures.ViewDataDictionary(
                new Microsoft.AspNetCore.Mvc.ModelBinding.EmptyModelMetadataProvider(),
                new Microsoft.AspNetCore.Mvc.ModelBinding.ModelStateDictionary())
            {
                ["Error"] = "系统内部错误，请稍后重试"
            }
        };

        context.ExceptionHandled = true;
    }
}
