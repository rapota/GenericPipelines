namespace GenericPipelines.Tests;

internal static class CodeBase
{
    public const string CommonCode = """
#nullable enable

using System;
using System.Threading;
using System.Threading.Tasks;

namespace GenericPipelines
{
    public interface IRequestHandler<in TRequest, TResponse>
    {
        Task<TResponse> HandleAsync(TRequest request, CancellationToken ct = default);
    }

    public interface IRequestHandler<in TRequest>
    {
        Task HandleAsync(TRequest request, CancellationToken ct = default);
    }

namespace Middlewares
{
    public interface IGenericPipeline;

    [AttributeUsage(AttributeTargets.Class)]
    public sealed class PipelineDecoratedAttribute<TPipelineType>  : Attribute
        where TPipelineType : IGenericPipeline;


    public delegate Task<TResponse> NextMiddlewareDelegate<in TRequest, TResponse>(TRequest request, CancellationToken ct);
    public interface IMiddleware<TRequest, TResponse>
    {
        Task<TResponse> InvokeAsync(TRequest request, NextMiddlewareDelegate<TRequest, TResponse> next, CancellationToken ct);
    }

    public delegate Task NextVoidMiddlewareDelegate<in TRequest>(TRequest request, CancellationToken ct);
    public interface IMiddleware<TRequest>
    {
        Task InvokeAsync(TRequest request, NextVoidMiddlewareDelegate<TRequest> next, CancellationToken ct);
    }

    public class Pipeline<TRequest> : IGenericPipeline
    {
        public Pipeline(params IMiddleware<TRequest>[] middlewares)
        {
        }

        public IRequestHandler<TRequest> DecorateHandler(IRequestHandler<TRequest> requestHandler)
        {
            throw new NotImplementedException();
        }
    }

    public class Pipeline<TRequest, TResponse> : IGenericPipeline
    {
        public Pipeline(params IMiddleware<TRequest, TResponse>[] middlewares)
        {
        }

        public IRequestHandler<TRequest, TResponse> DecorateHandler(IRequestHandler<TRequest, TResponse> requestHandler)
        {
            throw new NotImplementedException();
        }
    }
}
}
""";
}