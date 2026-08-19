namespace GodotExportValidator;

internal static class AttributeHelper
{
    public const string Attributes = @"
namespace GodotExportValidator
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal sealed class ExportValidation : Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal sealed class ExportNullCheck : Attribute { }
}
";

    private const string Namespace = nameof(GodotExportValidator);

    public const string ExportValidationName = "ExportValidation";
    public const string ExportNullCheckName = "ExportNullCheck";

    public const string ExportValidationFullName = $@"{Namespace}.{ExportValidationName}";
    public const string ExportNullCheckFullName = $@"{Namespace}.{ExportNullCheckName}";
}


