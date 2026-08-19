//HintName: GodotExportValidatorAttributes.g.cs

using System;
namespace GodotExportValidator
{
    [AttributeUsage(AttributeTargets.Class)]
    internal sealed class ExportValidation : Attribute { }

    [AttributeUsage(AttributeTargets.Field)]
    internal sealed class ExportNullCheck : Attribute { }
}
