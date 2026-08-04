using VerifyXunit;

namespace GodotExportValidator.Tests;

public class ExportValidatorSnapshotTests
{
    [Fact]
    public Task GeneratesExportNullValidationCorrectly()
    {
        var source = @"
using Godot;
using GodotExportValidator;

namespace Tests;

[ExportValidation]
public partial class GodotComponent : Node3D
{
    [Export]
    [ExportNullCheck]
    private RayCast3D _rayCast;

    public override void _Ready()
    {
        ValidateExports();
    }
}
";

        return TestHelper.Verify(source);
    }
}