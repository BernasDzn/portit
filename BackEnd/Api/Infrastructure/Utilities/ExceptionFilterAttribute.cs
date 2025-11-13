using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Api.Infrastructure.Filters
{
    public class GlobalExceptionFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            if (context.Exception is ArgumentException argEx)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = argEx.Message
                });
                context.ExceptionHandled = true;
                return;
            }

            if (context.Exception is InvalidOperationException invalidEx)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    Message = invalidEx.Message
                });
                context.ExceptionHandled = true;
                return;
            }

            context.Result = new ObjectResult(new
            {
                Message = "An unexpected error occurred."
            })
            {
                StatusCode = 500
            };

            context.ExceptionHandled = true;
        }
    }
}
