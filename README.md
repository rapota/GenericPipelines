# GenericPipelines

A lightweight .NET library which allow you to build your own pipelines to enhance your handlers with middlewares.

With this tool can implement similar to ASP.NET Core Middleware pattern.

Fundamental principles:
* lightweight with no dependencies
* minimal code impact
* DI used to resolve all handlers and middlewares/pipelines
* no reflection used
* code generation is used
* AOT compatible

## Installing GenericPipelines

You should install [Lesyk.GenericPipelines](https://www.nuget.org/packages/Lesyk.GenericPipelines/) with NuGet:

    Install-Package Lesyk.GenericPipelines

### Using Contracts-Only Package

To reference only the inerfaces:
* IRequestHandler
* IMiddleware

Add a package reference to [Lesyk.GenericPipelines.Middlewares](https://www.nuget.org/packages/Lesyk.GenericPipelines.Middlewares/).
This package is useful in scenarios where your contracts are in a separate project from handler's implementations.

## Creating request handler

Pipelines has two kinds of handlers to work with:
* request/response handlers
* request only handlers

### Define handler contract

#### Request/response contract:
```
public record GetByIdQuery(int Id);

public interface IGetByIdQueryHandler : IRequestHandler<GetByIdQuery, Foo?>;
```

#### Request only contract:
```
public record DeleteFooCommand(int Id);

public interface IDeleteFooCommandHandler : IRequestHandler<DeleteFooCommand>;
```

### Define handler implementation
Just implement defined interfaces.

#### Request/response handler implentation:
```
internal sealed class GetByIdQueryHandler : IGetByIdQueryHandler
{
    public Task<Foo?> HandleAsync(GetByIdQuery request, CancellationToken ct = default)
    {
        return Task.FromResult(new Foo());
    }
}
```

#### Request only implentation:
```
internal sealed class DeleteFooCommandHandler : IDeleteFooCommandHandler
{
    public Task HandleAsync(DeleteFooCommand request, CancellationToken ct = default)
    {
         return Task.CompletedTask;
    }
}
```

## Creating generic middleware

Generic generic middlewares can be reused for different pipelines.
Pipelines has two kinds of middlewares to work with:
* request/response middlewares
* request only middlewares

### Define request/response middleware
Define class that implements `IMiddleware<,>` interface.
```
internal sealed class Middleware1<TRequest, TResponse> : IMiddleware<TRequest, TResponse>
{
    public async Task<TResponse> InvokeAsync(TRequest request, NextMiddlewareDelegate<TRequest, TResponse> next, CancellationToken ct)
    {
        return await next(request, ct);
    }
}    
```

### Define request only middleware
Define class that implements `IMiddleware<>` interface.
```
internal sealed class Middleware1<TRequest> : IMiddleware<TRequest>
{
    public async Task InvokeAsync(TRequest request, NextVoidMiddlewareDelegate<TRequest> next, CancellationToken ct)
    {
        await next(request, ct);
    }
}
```

## Creating generic pipelines

Generic generic pipelines can be reused for different handlers.
There are two kinds of pipelines:
* request/response pipelines
* request only pipelines

### Define request/response pipeline
Define class derived from `Pipeline<,>`.

```
internal sealed class SampleQueryPipeline<TRequest, TResponse> : Pipeline<TRequest, TResponse>
{
    public SampleQueryPipeline(
        Middleware1<TRequest, TResponse> middleware1,
        Middleware2<TRequest, TResponse> middleware2)
        : base(
            // Parameter's order represents nesting hierarchy of middlewares.
            middleware1,
            middleware2)
    {
    }
}
```

### Define request only pipeline
Define class derived from `Pipeline<>`.
```
internal sealed class SampleCommandPipeline<TRequest> : Pipeline<TRequest>
{
    public SampleCommandPipeline(
        Middleware1<TRequest> middleware1,
        Middleware2<TRequest> middleware2)
        : base(
            // Parameter's order represents nesting hierarchy of middlewares.
            middleware1,
            middleware2)
    {
    }
}
```

## Decorate handler with generic pipeline
Just mark handler's implementation with `PipelineDecorated<>` attribute.
```
[PipelineDecorated<SampleQueryPipeline<GetByIdQuery, Foo>>]
internal sealed class GetByIdQueryHandler : IGetByIdQueryHandler
```
or
```
[PipelineDecorated<SampleCommandPipeline<DeleteFooCommand>>]
internal sealed class DeleteFooCommandHandler : IDeleteFooCommandHandler
```

## Registration in DI container
All Middlewares, Pipelines, Reqest handlers should be registered in DI.

### Register middlewares
Generic middleware registration example: `services.AddTransient(typeof(Middleware1<,>))`

### Register pipelines
Generic pipeline registration example: `services.AddTransient(typeof(SampleQueryPipeline<,>))`

### Register handlers
Handler registration example: `services.RegisterDecoratedRequestHandlers()`

Package will generate static class into target project with DI registration logic to register all handlers in project marked with `PipelineDecorated<>` attribute.
You shoul call `RegisterDecoratedRequestHandlers()` method at DI initialization. Generated class will be similar to this:
```
namespace GenericPipelines.Generated
{
    internal static class RegistrationExtensions
    {
        public static IServiceCollection RegisterDecoratedRequestHandlers(this IServiceCollection services)
        {
            // Rester handler #1
            // Rester handler #2
            // ...
            return services;
        }
    }
}
```

## Samples

Example AOT project with [GenericPipelines samples](https://github.com/rapota/GenericPipelines-Example).