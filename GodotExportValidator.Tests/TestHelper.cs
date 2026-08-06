using System.Collections.Immutable;
using System.Reflection;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotExportValidator.Tests;

public static class TestHelper
{
    
    private static readonly MetadataReference SystemRuntimeReference =
        MetadataReference.CreateFromFile(Assembly.Load("System.Runtime, Version=8.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a").Location);
    private static readonly MetadataReference NetStandard =
        MetadataReference.CreateFromFile(Assembly.Load("netstandard, Version=2.0.0.0, Culture=neutral, PublicKeyToken=cc7b13ffcd2ddd51").Location);
    private static readonly MetadataReference SystemPrivateCoreLib =
        MetadataReference.CreateFromFile(typeof(object).GetTypeInfo().Assembly.Location);
    private static readonly MetadataReference Generator =
        MetadataReference.CreateFromFile(typeof(ExportValidatorGenerator).GetTypeInfo().Assembly.Location);

    public static Task Verify(string source)
    {
        // Parse and compile code
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree },
            references: new[] { SystemRuntimeReference, NetStandard, SystemPrivateCoreLib, Generator });

        // Run source generator
        var generator = new ExportValidatorGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        var results = driver.GetRunResult();

        // Test output
        return Verifier.Verify(results);
    }
}