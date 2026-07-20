using System;

namespace GodotExportValidator;

[AttributeUsage(AttributeTargets.Class)]
public class ExportValidation : Attribute { }

[AttributeUsage(AttributeTargets.Field)]
public class ExportNullCheck : Attribute { }
