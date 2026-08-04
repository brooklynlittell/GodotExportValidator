using System.Collections.Immutable;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;

namespace GodotExportValidator.Tests;

public static class TestHelper
{
    public static Task Verify(string source)
    {
        // Parse and compile code
        var syntaxTree = CSharpSyntaxTree.ParseText(source);
        var compilation = CSharpCompilation.Create(
            assemblyName: "Tests",
            syntaxTrees: new[] { syntaxTree });

        // Run source generator
        var generator = new ExportValidatorGenerator();
        GeneratorDriver driver = CSharpGeneratorDriver.Create(generator);
        driver = driver.RunGenerators(compilation);

        var results = driver.GetRunResult();

        // Test output
        return Verifier.Verify(results);
    }
}