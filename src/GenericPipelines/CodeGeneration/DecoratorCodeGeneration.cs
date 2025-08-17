using System.CodeDom.Compiler;
using GenericPipelines.Generators;

namespace GenericPipelines.CodeGeneration;

internal static class DecoratorCodeGeneration
{
    public static string GenerateDecorator(HandlerMetadata handlerMetadata)
    {
        return FileGenerator.GenerateFile(code => GenerateFile(code, handlerMetadata));
    }

    private static void GenerateFile(IndentedTextWriter source, HandlerMetadata handlerMetadata)
    {
        source.WriteLine("using System;");
        source.WriteLine("using System.Threading;");
        source.WriteLine("using System.Threading.Tasks;");
        source.WriteLine("using Microsoft.Extensions.DependencyInjection;");
        source.WriteLine();

        source.WriteLine("namespace GenericPipelines.Generated.Decorators");
        source.WriteLine("{");
        source.Indent++;

        GenerateClass(source, handlerMetadata);

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateClass(IndentedTextWriter source, HandlerMetadata handlerMetadata)
    {
        var className = typeof(DecoratorGenerator).FullName;
        var assemblyVersion = typeof(DecoratorGenerator).Assembly.GetName().Version.ToString();

        source.WriteLine("[global::System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", className, assemblyVersion);
        source.WriteLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        GenerateDecorator(source, handlerMetadata);
    }

    private static void GenerateDecorator(IndentedTextWriter source, HandlerMetadata handler)
    {
        string decoratorName = handler.Type.FullName.Replace('.', '_') + "_Decorator";

        source.WriteLine($"internal sealed class {decoratorName}");

        source.Indent++;
        source.WriteLine($": {handler.Interface.Type}");
        source.Indent--;

        source.WriteLine("{");
        source.Indent++;

        GenerateFields(source, handler);
        source.WriteLine();

        GenerateConstructor(source, handler, decoratorName);
        source.WriteLine();

        GenerateHandlerMethod(source, handler);
        source.WriteLine();

        GenerateRegistrationMethod(source, handler, decoratorName);

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateFields(IndentedTextWriter source, HandlerMetadata handler)
    {
        source.WriteLine(
            "private readonly IRequestHandler<{0}> _requestHandler;",
            Mappings.Format(handler.Interface.RequestResponseTypes));
    }

    private static void GenerateConstructor(IndentedTextWriter source, HandlerMetadata handler, string decoratorName)
    {
        source.WriteLine($"public {decoratorName}(");
        source.Indent++;

        source.WriteLine($"{handler.AttributeMetadata.PipelineType} pipeline,");

        source.WriteLine($"{handler.Type.FullName} requestHandler)");

        source.Indent--;
        source.WriteLine("{");

        source.Indent++;
        source.WriteLine("_requestHandler = pipeline.DecorateHandler(requestHandler);");
        source.Indent--;

        source.WriteLine("}");
    }

    private static void GenerateHandlerMethod(IndentedTextWriter source, HandlerMetadata handler)
    {
        if (string.IsNullOrEmpty(handler.Interface.RequestResponseTypes.ResponseType))
        {
            source.WriteLine(
                "public Task HandleAsync({0} request, CancellationToken ct = default)",
                handler.Interface.RequestResponseTypes.RequestType);
        }
        else
        {
            source.WriteLine(
                "public Task<{0}> HandleAsync({1} request, CancellationToken ct = default)",
                handler.Interface.RequestResponseTypes.ResponseType,
                handler.Interface.RequestResponseTypes.RequestType);
        }

        source.WriteLine("{");
        source.Indent++;

        source.WriteLine("return _requestHandler.HandleAsync(request, ct);");

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateRegistrationMethod(IndentedTextWriter source, HandlerMetadata handler, string decoratorName)
    {
        source.WriteLine("public static void RegisterDecoratedHandler(IServiceCollection services)");
        source.WriteLine("{");
        source.Indent++;

        source.WriteLine("services");
        source.Indent++;
        source.WriteLine($".AddTransient<{handler.Type.FullName}>()");
        source.WriteLine($".AddTransient<{handler.Interface.Type}, {decoratorName}>();");
        source.Indent--;

        source.Indent--;
        source.WriteLine("}");
    }
}