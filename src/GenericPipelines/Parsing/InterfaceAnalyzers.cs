using System.Collections.Immutable;
using Microsoft.CodeAnalysis;

namespace GenericPipelines.Parsing;

internal static class InterfaceAnalyzers
{
    private const string HandlerInterfaceNamespace = "GenericPipelines";
    private const string HandlerInterfaceName = "IRequestHandler";

    public static InterfaceMetadata? TryGetInterfaceMetadata(INamedTypeSymbol handlerSymbol)
    {
        foreach (INamedTypeSymbol interfaceSymbol in handlerSymbol.Interfaces)
        {
            RequestResponseMetadata? requestResponseTypes = TryGetRequestResponseTypesRecursive(interfaceSymbol);
            if (requestResponseTypes != null)
            {
                return new InterfaceMetadata(
                    interfaceSymbol.ToString(),
                    (RequestResponseMetadata)requestResponseTypes);
            }
        }

        return null;
    }

    private static bool IsIHandler(INamedTypeSymbol interfaceSymbol) =>
        interfaceSymbol.ContainingNamespace.Name == HandlerInterfaceNamespace
        && interfaceSymbol.Name == HandlerInterfaceName;

    private static RequestResponseMetadata? TryGetRequestResponseTypes(INamedTypeSymbol interfaceSymbol)
    {
        if (!IsIHandler(interfaceSymbol))
        {
            return null;
        }

        ImmutableArray<ITypeSymbol> arguments = interfaceSymbol.TypeArguments;
        if (arguments.Length < 1)
        {
            return null;
        }

        return arguments.Length == 1
            ? new RequestResponseMetadata(
                arguments[0].ToString(),
                null)
            : new RequestResponseMetadata(
                arguments[0].ToString(),
                arguments[1].ToString());
    }

    private static RequestResponseMetadata? TryGetRequestResponseTypesRecursive(INamedTypeSymbol interfaceSymbol)
    {
        foreach (INamedTypeSymbol symbol in interfaceSymbol.Interfaces)
        {
            RequestResponseMetadata? requestResponseTypes = TryGetRequestResponseTypesRecursive(symbol);
            if (requestResponseTypes != null)
            {
                return requestResponseTypes;
            }
        }

        return TryGetRequestResponseTypes(interfaceSymbol);
    }
}