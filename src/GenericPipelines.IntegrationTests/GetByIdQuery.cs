using GenericPipelines.Middlewares;

namespace GenericPipelines.IntegrationTests;

public record Todo(int Id, string? Title, DateOnly? DueBy = null, bool IsComplete = false);

public record GetByIdRequest(Guid Id);

public interface IGetByIdRequestHandler : IRequestHandler<GetByIdRequest, Todo>;

[PipelineDecorated<DefaultPipeline<GetByIdRequest, Todo>>]
internal sealed class GetByIdRequestHandler : IGetByIdRequestHandler
{
    public Task<Todo> HandleAsync(GetByIdRequest request, CancellationToken ct = default)
    {
        Todo result = new(1, "Walk the dog");
        return Task.FromResult(result);
    }
}
