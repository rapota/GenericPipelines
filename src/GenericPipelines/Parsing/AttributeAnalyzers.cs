using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace GenericPipelines.Parsing;

internal static class AttributeAnalyzers
{
    private const string AttributeNamespace = "GenericPipelines.Middlewares";
    private const string AttributeName = "PipelineDecoratedAttribute";
    public const string AttributeFullName = AttributeNamespace + "." + AttributeName + "`1";

    public static AttributeMetadata? TryGetAttributeMetadata(INamedTypeSymbol handlerSymbol)
    {
        ImmutableArray<AttributeData> attributes = handlerSymbol.GetAttributes();
        AttributeData? usePipelineAttribute = attributes.FirstOrDefault(IsUsePipelineAttribute);
        if (usePipelineAttribute == null)
        {
            return null;
        }

        ITypeSymbol? pipelineTypeSymbol = usePipelineAttribute.AttributeClass?.TypeArguments[0];
        if (pipelineTypeSymbol == null)
        {
            return null;
        }

        return new AttributeMetadata(pipelineTypeSymbol.ToString());
    }

    private static bool IsUsePipelineAttribute(AttributeData attributeData)
    {
        INamedTypeSymbol? symbol = attributeData.AttributeClass;
        if (symbol == null)
        {
            return false;
        }

        return symbol.ContainingNamespace.ToString() == AttributeNamespace
            && symbol.Name == AttributeName;
    }
}
