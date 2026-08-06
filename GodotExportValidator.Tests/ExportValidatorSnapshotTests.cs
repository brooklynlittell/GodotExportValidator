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
    
    [Fact]
    public Task GeneratesMultipleExportNullValidationsCorrectly()
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

    [Export]
    [ExportNullCheck]
    private Node3D _node;

    public override void _Ready()
    {
        ValidateExports();
    }
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task GeneratesCustomExportNullValidationCorrectly()
    {
        var source = @"
using Godot;
using GodotExportValidator;

namespace Tests;

public partial class CustomComponent : Node3D { }

[ExportValidation]
public partial class GodotComponent : Node3D
{
    [Export]
    [ExportNullCheck]
    private CustomComponent _customComponent;

    public override void _Ready()
    {
        ValidateExports();
    }
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task SkipsUnattributedValidationsCorrectly()
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

    [Export]
    private Node3D _nullNode;

    [Export]
    [ExportNullCheck]
    private Node3D _node;

    [Export]
    private int _testCount;

    private int _testCount2;
    private Node3D _nonExportNode;

    public override void _Ready()
    {
        ValidateExports();
    }
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task SkipsUnattributedMemberCorrectly()
    {
        var source = @"
using Godot;
using GodotExportValidator;

namespace Tests;

[ExportValidation]
public partial class GodotComponent : Node3D
{
    [Export]
    private Node3D _nullNode;
}
";

        return TestHelper.Verify(source);
    }
    
    [Fact]
    public Task SkipsUnattributedClassCorrectly()
    {
        var source = @"
using Godot;
using GodotExportValidator;

namespace Tests;

public partial class GodotComponent : Node3D
{
    [Export]
    private Node3D _nullNode;
}
";

        return TestHelper.Verify(source);
    }
}