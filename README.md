# GodotExportValidator

Validates C# Godot exports to catch null values earlier.

Built using Godot 4.7, but should support Godot 4.5+

# Usage
1. Add Nuget Package to your Godot C# project
2. Follow the following example
## Example
```c#
// Player.cs

using GodotExportValidator;

[ExportValidation]                   // <- Class Attribute
public partial class Player : CharacterBody3D
{
    [Export]
    [ExportNullCheck]                // <- Export Attribute
    private HealthComponent _healthComponent;
    
    [Export]
    [ExportNullCheck]                // <- Export Attribute
    private Camera3D _camera;
    
    public override void _Ready()
    {
        ValidateExports();           // <- Generated Function
    }
}
```

<details>

<summary>Generated Code</summary>

```c#
// Player.g.cs
// This file will not appear in your editor but is consumed by the compiler

public partial class Player
{
    private void ValidateExports()
    {
        if (_healthComponent == null)
        {
            GD.PushError("Null Export: Player _healthComponent HealthComponent");
        }
        if (_camera == null)
        {
            GD.PushError("Null Export: Player _camera Camera3D");
        }
    }
}
```

</details>

# Why?
Most sample code for Godot references scene nodes using `GetNode<T>("NodeName")`. I don't like this because it conflates editor context with script context. Scripts should not need to track editor node names, and editor organizational changes should not impact script function.

Whenever my scripts need to reference nodes in their scene, I instead `[Export]` the nodes. This makes it clear in the editor when a certain node is required, while at the same time separating script naming conventions from the editor. However, it can be easy to forget to hook-up a node export, or, depending on the editor action, accidentally disconnect a node export. When this happens scripts will stop working but not neccessarily be obvious.

To help catch these errors faster, I check for Node null values in `_Ready`. This leads to boilerplate bloat this is better handled by a source generator. Thus, this library :)