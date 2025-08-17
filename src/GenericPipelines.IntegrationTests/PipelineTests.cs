using GenericPipelines.Generated;
using Microsoft.Extensions.DependencyInjection;

namespace GenericPipelines.IntegrationTests;

public class PipelineTests
{
    [Fact]
    public void QueryTest()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddTransient(typeof(EmptyQueryMiddleware<,>));
        services.AddTransient(typeof(DefaultPipeline<,>));
        services.RegisterDecoratedRequestHandlers();

        using ServiceProvider provider = services.BuildServiceProvider();
        IGetByIdRequestHandler requestHandler = provider.GetRequiredService<IGetByIdRequestHandler>();

        Assert.NotEqual(typeof(GetByIdRequestHandler), requestHandler.GetType());
    }
    
    [Fact]
    public void EventTest()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddTransient(typeof(EmptyEvetMiddleware<>));
        services.AddTransient(typeof(DefaultEventPipeline<>));
        services.RegisterDecoratedRequestHandlers();

        using ServiceProvider provider = services.BuildServiceProvider();
        ISimpleEventHandler eventHandler = provider.GetRequiredService<ISimpleEventHandler>();

        Assert.NotEqual(typeof(SimpleEventHandler), eventHandler.GetType());
    }
}