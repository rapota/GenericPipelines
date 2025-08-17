using Microsoft.CodeAnalysis;

namespace GenericPipelines;

internal static class Mappings
{
    public static TypeMetadata ToNamedTypeMetadata(INamedTypeSymbol namedTypeSymbol) =>
        new(namedTypeSymbol.Name, namedTypeSymbol.ToString());

    public static string Format(RequestResponseMetadata requestResponseMetadata) =>
        requestResponseMetadata.ResponseType == null
        ? requestResponseMetadata.RequestType
        : $"{requestResponseMetadata.RequestType}, {requestResponseMetadata.ResponseType}";
}
