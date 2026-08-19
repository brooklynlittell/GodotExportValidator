//HintName: GodotExportValidatorAttributes.g.cs

namespace GodotExportValidator
{
    [System.AttributeUsage(System.AttributeTargets.Class)]
    internal sealed class ExportValidation : Attribute { }

    [System.AttributeUsage(System.AttributeTargets.Field)]
    internal sealed class ExportNullCheck : Attribute { }
}
