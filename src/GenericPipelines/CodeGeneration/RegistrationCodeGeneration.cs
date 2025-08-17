using System.CodeDom.Compiler;
using GenericPipelines.Generators;

namespace GenericPipelines.CodeGeneration;

internal static class RegistrationCodeGeneration
{
    public static string GenerateDecorator(IEnumerable<HandlerMetadata> handlers)
    {
        return FileGenerator.GenerateFile(code => GenerateFile(code, handlers));
    }

    private static void GenerateFile(IndentedTextWriter source, IEnumerable<HandlerMetadata> handlers)
    {
        //source.WriteLine("using System;");
        //source.WriteLine("using System.Threading;");
        //source.WriteLine("using System.Threading.Tasks;");
        source.WriteLine("using Microsoft.Extensions.DependencyInjection;");
        source.WriteLine();

        source.WriteLine("namespace GenericPipelines.Generated");
        source.WriteLine("{");
        source.Indent++;

        GenerateClass(source, handlers);

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateClass(IndentedTextWriter source, IEnumerable<HandlerMetadata> handlers)
    {
        var className = typeof(DecoratorGenerator).FullName;
        var assemblyVersion = typeof(DecoratorGenerator).Assembly.GetName().Version.ToString();

        source.WriteLine("[global::System.CodeDom.Compiler.GeneratedCode(\"{0}\", \"{1}\")]", className, assemblyVersion);
        source.WriteLine("[global::System.Diagnostics.CodeAnalysis.ExcludeFromCodeCoverage]");
        source.WriteLine("internal static class RegistrationExtensions");
        source.WriteLine("{");
        source.Indent++;

        GenerateRegistrator(source, handlers);

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateRegistrator(IndentedTextWriter source, IEnumerable<HandlerMetadata> handlers)
    {
        source.WriteLine("public static IServiceCollection RegisterDecoratedRequestHandlers(this IServiceCollection services)");
        source.WriteLine("{");
        source.Indent++;

        GenerateHandlers(source, handlers);
        source.WriteLine("return services;");

        source.Indent--;
        source.WriteLine("}");
    }

    private static void GenerateHandlers(IndentedTextWriter source, IEnumerable<HandlerMetadata> handlers)
    {
        foreach (HandlerMetadata handler in handlers)
        {
            string decoratorName = handler.Type.FullName.Replace('.', '_') + "_Decorator";
            source.WriteLine($"Decorators.{decoratorName}.RegisterDecoratedHandler(services);");
        }
    }
}