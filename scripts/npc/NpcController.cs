using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

[Tool]
public partial class NpcController : Node3D
{
    [Export]
    public AnimationPlayer animationPlayer
    {
        private set
        {
            _animationPlayer = value;
            NotifyPropertyListChanged();
            PlayDefaultAnimation();
        }
        get => _animationPlayer;
    }
    [Export]
    public string DefaultAnimation
    {
        private set
        {
            _defaultAnimation = value;
            PlayDefaultAnimation();
        }
        get => _defaultAnimation;
    }

    [Export]
    public Skeleton3D Skeleton
    {
        get => _skeleton;
        set
        {
            _skeleton = value;
            NotifyPropertyListChanged();
            UpdateMeshVisibility();
        }
    }

    [Export]
    public string CurrentMesh
    {
        get => _currentMesh;
        set
        {
            _currentMesh = value;
            UpdateMeshVisibility();
        }
    }

    [Export]
    public bool Refresh
    {
        get => false;
        set
        {
            if (!value) return;
            NotifyPropertyListChanged();
        }
    }

    private AnimationPlayer _animationPlayer;
    private string _defaultAnimation;
    private Skeleton3D _skeleton;
    private string _currentMesh = "";

    public override void _Ready()
    {
        PlayDefaultAnimation();
        UpdateMeshVisibility();
    }

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if (property["name"].AsStringName() == PropertyName.CurrentMesh)
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = _skeleton != null
                ? string.Join(",", _skeleton.GetChildren().Select(child => child.Name))
                : "";
        }

        if (property["name"].AsStringName() == PropertyName.DefaultAnimation)
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = _animationPlayer != null
                ? string.Join(",", _animationPlayer.GetAnimationList())
                : "";
        }
    }

    private void PlayDefaultAnimation()
    {
        _animationPlayer.Play(_defaultAnimation, -1, 0.25f);
    }

    private void UpdateMeshVisibility()
    {
        if (_skeleton == null) return;

        foreach (Node child in _skeleton.GetChildren())
        {
            if (child is Node3D node3D)
                node3D.Visible = node3D.Name == _currentMesh;
        }
    }
}
