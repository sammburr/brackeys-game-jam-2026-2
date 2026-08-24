using Godot;
using System;

[Tool]
public partial class WindMillSails : MeshInstance3D
{
    public override void _Process(double delta)
    {
        RotateX(0.25f * (float)delta);
    }
}
