using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Abp.AspNetCore.App.Controllers;
using Abp.AspNetCore.Mvc.ExceptionHandling;
using Abp.UI;
using Castle.Core.Logging;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.RazorPages.Infrastructure;
using Microsoft.AspNetCore.Routing;
using NSubstitute;
using Shouldly;
using Xunit;

namespace Abp.AspNetCore.Tests;

/// <summary>
/// Tests for https://github.com/aspnetboilerplate/aspnetboilerplate/issues/7210:
/// a request that is aborted by the client is not an application error, so it should neither be
/// logged as an error nor be wrapped into an error response.
/// </summary>
public class ExceptionFilter_ClientCancellation_Tests : AppTestBase
{
    [Fact]
    public void AbpExceptionFilter_Should_Not_Log_Or_Wrap_Client_Cancellation()
    {
        var context = CreateExceptionContext(new OperationCanceledException(), requestAborted: true);
        var logger = CreateFakeLogger();

        var filter = IocManager.Resolve<AbpExceptionFilter>();
        filter.Logger = logger;
        filter.OnException(context);

        logger.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<Exception>());
        context.Exception.ShouldBeOfType<OperationCanceledException>(); // Not handled by ABP
        context.Result.ShouldBeNull();
        context.HttpContext.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
    }

    [Fact]
    public void AbpExceptionFilter_Should_Log_And_Wrap_Cancellation_Of_A_Live_Request()
    {
        var context = CreateExceptionContext(new OperationCanceledException(), requestAborted: false);
        var logger = CreateFakeLogger();

        var filter = IocManager.Resolve<AbpExceptionFilter>();
        filter.Logger = logger;
        filter.OnException(context);

        logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>());
        context.Exception.ShouldBeNull(); // Handled by ABP
        context.Result.ShouldBeOfType<ObjectResult>();
        context.HttpContext.Response.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public void AbpExceptionFilter_Should_Log_And_Wrap_Other_Exceptions_Of_An_Aborted_Request()
    {
        var context = CreateExceptionContext(new UserFriendlyException("Test error"), requestAborted: true);
        var logger = CreateFakeLogger();

        var filter = IocManager.Resolve<AbpExceptionFilter>();
        filter.Logger = logger;
        filter.OnException(context);

        logger.Received(1).Warn(Arg.Any<string>(), Arg.Any<Exception>());
        context.Exception.ShouldBeNull(); // Handled by ABP
        context.Result.ShouldBeOfType<ObjectResult>();
        context.HttpContext.Response.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    [Fact]
    public async Task AbpExceptionPageFilter_Should_Not_Log_Or_Wrap_Client_Cancellation()
    {
        var exception = new OperationCanceledException();
        var (executingContext, executedContext) = CreatePageHandlerContexts(exception, requestAborted: true);
        var logger = CreateFakeLogger();

        var filter = IocManager.Resolve<AbpExceptionPageFilter>();
        filter.Logger = logger;
        await filter.OnPageHandlerExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        logger.DidNotReceive().Error(Arg.Any<string>(), Arg.Any<Exception>());
        executedContext.Exception.ShouldBe(exception); // Not handled by ABP
        executedContext.Result.ShouldBeNull();
        executedContext.HttpContext.Response.StatusCode.ShouldBe((int)HttpStatusCode.OK);
    }

    [Fact]
    public async Task AbpExceptionPageFilter_Should_Log_And_Wrap_Cancellation_Of_A_Live_Request()
    {
        var (executingContext, executedContext) =
            CreatePageHandlerContexts(new OperationCanceledException(), requestAborted: false);
        var logger = CreateFakeLogger();

        var filter = IocManager.Resolve<AbpExceptionPageFilter>();
        filter.Logger = logger;
        await filter.OnPageHandlerExecutionAsync(executingContext, () => Task.FromResult(executedContext));

        logger.Received(1).Error(Arg.Any<string>(), Arg.Any<Exception>());
        executedContext.Exception.ShouldBeNull(); // Handled by ABP
        executedContext.Result.ShouldBeOfType<ObjectResult>();
        executedContext.HttpContext.Response.StatusCode.ShouldBe((int)HttpStatusCode.InternalServerError);
    }

    private static ILogger CreateFakeLogger()
    {
        return Substitute.For<ILogger>();
    }

    private static HttpContext CreateHttpContext(bool requestAborted)
    {
        var httpContext = new DefaultHttpContext();

        httpContext.Request.Scheme = "http";
        httpContext.Request.Host = new HostString("localhost");
        httpContext.Request.Method = "GET";
        httpContext.Request.Path = "/WrapResultTest/Get";

        if (requestAborted)
        {
            var cancellationTokenSource = new CancellationTokenSource();
            cancellationTokenSource.Cancel();
            httpContext.RequestAborted = cancellationTokenSource.Token;
        }

        return httpContext;
    }

    private static ExceptionContext CreateExceptionContext(Exception exception, bool requestAborted)
    {
        var actionDescriptor = new ControllerActionDescriptor
        {
            ControllerTypeInfo = typeof(WrapResultTestController).GetTypeInfo(),
            MethodInfo = typeof(WrapResultTestController).GetMethod(nameof(WrapResultTestController.Get))
        };

        var actionContext = new ActionContext(CreateHttpContext(requestAborted), new RouteData(), actionDescriptor);

        return new ExceptionContext(actionContext, new List<IFilterMetadata>())
        {
            Exception = exception
        };
    }

    private static (PageHandlerExecutingContext, PageHandlerExecutedContext) CreatePageHandlerContexts(
        Exception exception,
        bool requestAborted)
    {
        var handlerMethod = new HandlerMethodDescriptor
        {
            HttpMethod = "GET",
            MethodInfo = typeof(TestPageModel).GetMethod(nameof(TestPageModel.OnGet))
        };

        var actionContext = new ActionContext(
            CreateHttpContext(requestAborted),
            new RouteData(),
            new CompiledPageActionDescriptor()
        );

        var pageContext = new PageContext(actionContext);
        var pageModel = new TestPageModel();

        var executingContext = new PageHandlerExecutingContext(
            pageContext,
            new List<IFilterMetadata>(),
            handlerMethod,
            new Dictionary<string, object>(),
            pageModel
        );

        var executedContext = new PageHandlerExecutedContext(
            pageContext,
            new List<IFilterMetadata>(),
            handlerMethod,
            pageModel
        )
        {
            Exception = exception
        };

        return (executingContext, executedContext);
    }

    private class TestPageModel : PageModel
    {
        public JsonResult OnGet()
        {
            return new JsonResult(42);
        }
    }
}
