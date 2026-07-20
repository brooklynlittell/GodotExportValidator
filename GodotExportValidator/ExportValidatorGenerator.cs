using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace GodotExportValidator;

[Generator]
public class ExportValidatorGenerator : IIncrementalGenerator
{
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider.ForAttributeWithMetadataName(
            typeof(ExportValidation).FullName!,
            IsSyntaxTarget,
            GetSyntaxTarget
        );
        
        context.RegisterSourceOutput(syntaxProvider.Collect(), OnExecute);
    }

    private bool IsSyntaxTarget(SyntaxNode syntaxNode, CancellationToken cancellationToken)
    {
        return syntaxNode is ClassDeclarationSyntax;
    }

    private ClassExportValidation GetSyntaxTarget(GeneratorAttributeSyntaxContext context, CancellationToken cancellationToken)
    {
        var classSymbol = (ITypeSymbol)context.TargetSymbol;
        var nullCheckList = new List<ExportToNullValidate>();

        foreach (var memberSymbol in classSymbol.GetMembers())
        {
            if (memberSymbol is not IFieldSymbol fieldSymbol)
            {
                // We only validate fields
                continue;
            }
            
            foreach (var attribute in memberSymbol.GetAttributes())
            {
                cancellationToken.ThrowIfCancellationRequested();

                if (attribute.AttributeClass!.Name == nameof(ExportNullCheck))
                {
                    nullCheckList.Add(new ExportToNullValidate(
                        fieldSymbol.Name,
                        fieldSymbol.Type.Name));
                }
            }
        }

        return new ClassExportValidation(
            GetNamespace(context.TargetNode as ClassDeclarationSyntax),
            classSymbol.Name,
            nullCheckList.ToImmutableList());
    }

    private static void OnExecute(SourceProductionContext context, ImmutableArray<ClassExportValidation> classValidations)
    {
        foreach (var classValidation in classValidations)
        {
            context.CancellationToken.ThrowIfCancellationRequested();
            var result = GenerateValidationClass(classValidation);
            context.AddSource($"{classValidation.Namespace}.{classValidation.ClassName}.g.cs", SourceText.From(result, Encoding.UTF8));
        }
    }
    
    private static string GenerateValidationClass(ClassExportValidation classValidation)
    {
        var sb = new StringBuilder();
        sb.Append($@"
namespace {classValidation.Namespace};

using Godot;

public partial class {classValidation.ClassName}
{{
    private void ValidateExports()
    {{");

        foreach (var field in classValidation.ValidationList)
        {
            sb.Append($@"
        if ({field.FieldName} == null)
        {{
            GD.PushError(""Export null: {classValidation.ClassName} {field.FieldName} {field.FieldType}"");
        }}");
        }
        
        sb.Append($@"
    }}
}}");

        return sb.ToString();
    }

    private record struct ClassExportValidation(
        string Namespace,
        string ClassName,
        IImmutableList<ExportToNullValidate> ValidationList
    );

    private record ExportToNullValidate(string FieldName, string FieldType)
    {
        public readonly string FieldName = FieldName;
        public readonly string FieldType = FieldType;
    }
    
    // determine the namespace the class/enum/struct is declared in, if any
    static string GetNamespace(BaseTypeDeclarationSyntax syntax)
    {
        // If we don't have a namespace at all we'll return an empty string
        // This accounts for the "default namespace" case
        string nameSpace = string.Empty;

        // Get the containing syntax node for the type declaration
        // (could be a nested type, for example)
        SyntaxNode? potentialNamespaceParent = syntax.Parent;
    
        // Keep moving "out" of nested classes etc until we get to a namespace
        // or until we run out of parents
        while (potentialNamespaceParent != null &&
               potentialNamespaceParent is not NamespaceDeclarationSyntax
               && potentialNamespaceParent is not FileScopedNamespaceDeclarationSyntax)
        {
            potentialNamespaceParent = potentialNamespaceParent.Parent;
        }

        // Build up the final namespace by looping until we no longer have a namespace declaration
        if (potentialNamespaceParent is BaseNamespaceDeclarationSyntax namespaceParent)
        {
            // We have a namespace. Use that as the type
            nameSpace = namespaceParent.Name.ToString();
        
            // Keep moving "out" of the namespace declarations until we 
            // run out of nested namespace declarations
            while (true)
            {
                if (namespaceParent.Parent is not NamespaceDeclarationSyntax parent)
                {
                    break;
                }

                // Add the outer namespace as a prefix to the final namespace
                nameSpace = $"{namespaceParent.Name}.{nameSpace}";
                namespaceParent = parent;
            }
        }

        // return the final namespace
        return nameSpace;
    }
}
