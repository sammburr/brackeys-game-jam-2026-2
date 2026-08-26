using Godot;
using System;

[Tool]
public partial class DebugFlag : Node3D
{
    public override void _Ready()
    {
        if(!Engine.IsEditorHint())
            QueueFree();
    }

    public override void _Process(double delta)
    {
        RotateY(0.2f * (float)delta);
    }
}
