
using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.DependencyInjection;
using PublicContacts.App.Exceptions;
using Serilog;

namespace PublicContacts.Api.Filters
{
    public class HttpResponseExceptionFilter : IActionFilter, IOrderedFilter
    {
        public int Order { get; set; } = int.MaxValue - 10;

        public void OnActionExecuting(ActionExecutingContext context) { }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            if (context.Exception != null)
            {
                var pdFactory = context.HttpContext.RequestServices.GetService<ProblemDetailsFactory>();

                if (context.Exception is RequestException reqEx)
                {
                    var errors = new ModelStateDictionary();
                    if (reqEx.Key != null)
                        errors.AddModelError(reqEx.Key, reqEx.KeyMessage);

                    var pd = pdFactory.CreateValidationProblemDetails(context.HttpContext, errors, StatusCodes.Status400BadRequest, reqEx.Message);

                    context.Result = new ObjectResult(pd) { StatusCode = pd.Status, };
                }
                else if (context.Exception is Exception ex)
                {
                    var pd = pdFactory.CreateProblemDetails(context.HttpContext, StatusCodes.Status500InternalServerError, ex.Message);
                    context.Result = new ObjectResult(pd) { StatusCode = pd.Status, };
                    Log.Error(ex, ex.Message);
                }
            }

            context.ExceptionHandled = true;
        }
    }
}