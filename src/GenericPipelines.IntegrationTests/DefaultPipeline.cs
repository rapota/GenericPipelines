using GenericPipelines.Middlewares;

namespace GenericPipelines.IntegrationTests;

internal sealed class DefaultPipeline<TRequest, TResponse> : Pipeline<TRequest, TResponse>
{
    public DefaultPipeline(EmptyQueryMiddleware<TRequest, TResponse> empty)
        : base(empty)
    {
    }
}

internal sealed class EmptyQueryMiddleware<TRequest, TResponse>
    : IMiddleware<TRequest, TResponse>
{
    public Task<TResponse> InvokeAsync(TRequest request, NextMiddlewareDelegate<TRequest, TResponse> next, CancellationToken ct)
    {
        return next(request, ct);
    }
}