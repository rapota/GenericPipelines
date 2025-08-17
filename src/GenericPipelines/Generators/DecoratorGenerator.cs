using GenericPipelines.CodeGeneration;
using GenericPipelines.Parsing;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;
using System.Collections.Immutable;
using System.Text;

namespace GenericPipelines.Generators;

[Generator(LanguageNames.CSharp)]
public class DecoratorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
#if DEBUG
        //if (!Debugger.IsAttached)
        //{
        //    Debugger.Launch();
        //}
#endif

        IncrementalValuesProvider<HandlerMetadata> handlers = context.SyntaxProvider.ForAttributeWithMetadataName(
            fullyQualifiedMetadataName: AttributeAnalyzers.AttributeFullName,
            predicate: (syntaxNode, _) => syntaxNode is ClassDeclarationSyntax,
            transform: (syntaxContext, _) => HandlerAnalyzers.GetMetadata((INamedTypeSymbol)syntaxContext.TargetSymbol)
        )
        .Where(x => x.HasValue)
        .Select((x, _) => x!.Value);

        context.RegisterSourceOutput(handlers, AddGeneratedCode);

        IncrementalValueProvider<ImmutableArray<HandlerMetadata>> collectedHandlers = handlers.Collect();
        context.RegisterSourceOutput(collectedHandlers, AddRegistrationExtensions);
    }

    private static void AddGeneratedCode(SourceProductionContext context, HandlerMetadata handlerMetadata)
    {
        string code = DecoratorCodeGeneration.GenerateDecorator(handlerMetadata);
        SourceText sourceText = SourceText.From(code, Encoding.UTF8);

        string fileName = $"{handlerMetadata.Type.Name}Decorator.g.cs";
        context.AddSource(fileName, sourceText);
    }

    private void AddRegistrationExtensions(SourceProductionContext context, ImmutableArray<HandlerMetadata> handlers)
    {
        string code = RegistrationCodeGeneration.GenerateDecorator(handlers);
        SourceText sourceText = SourceText.From(code, Encoding.UTF8);

        string fileName = "GenericPipelinesExtensions.g.cs";
        context.AddSource(fileName, sourceText);
    }
}