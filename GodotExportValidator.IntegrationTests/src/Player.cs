using Godot;
//using GodotExportValidator;

namespace GodotExportValidator.IntegrationTests;

//[ExportValidation]
public partial class Player : Node3D
{
    [Export]
    private HealthComponent _healthComponent;

    [Export]
    private RayCast3D _rayCast;

    public override void _Ready()
    {
        //ValidateExports();
    }
}
