using GenericPipelines.Middlewares;

namespace GenericPipelines.IntegrationTests;

public record SimpleEvent;

public interface ISimpleEventHandler : IRequestHandler<SimpleEvent>;

[PipelineDecorated<DefaultEventPipeline<SimpleEvent>>]
internal sealed class SimpleEventHandler : ISimpleEventHandler
{
    public Task HandleAsync(SimpleEvent request, CancellationToken ct = default)
    {
        return Task.CompletedTask;
    }
}