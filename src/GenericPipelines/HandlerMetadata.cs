namespace GenericPipelines;

internal record struct HandlerMetadata(TypeMetadata Type, InterfaceMetadata Interface, AttributeMetadata AttributeMetadata);

internal record struct AttributeMetadata(string PipelineType);

internal record struct TypeMetadata(string Name, string FullName);

internal record struct InterfaceMetadata(string Type, RequestResponseMetadata RequestResponseTypes);

internal record struct RequestResponseMetadata(string RequestType, string? ResponseType);
