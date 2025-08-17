using Microsoft.CodeAnalysis;

namespace GenericPipelines.Parsing;

internal static class HandlerAnalyzers
{
    public static HandlerMetadata? GetMetadata(INamedTypeSymbol handlerSymbol)
    {
        InterfaceMetadata? interfaceMetadata = InterfaceAnalyzers.TryGetInterfaceMetadata(handlerSymbol);
        if (interfaceMetadata == null)
        {
            return null;
        }

        AttributeMetadata? attributeMetadata = AttributeAnalyzers.TryGetAttributeMetadata(handlerSymbol);
        if (attributeMetadata == null)
        {
            return null;
        }

        return new HandlerMetadata(
            Mappings.ToNamedTypeMetadata(handlerSymbol),
            (InterfaceMetadata)interfaceMetadata,
            (AttributeMetadata)attributeMetadata
        );
    }
}