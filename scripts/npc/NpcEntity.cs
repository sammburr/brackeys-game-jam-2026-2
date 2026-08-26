using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

[Tool]
public partial class NpcEntity : CharacterBody3D
{

    private enum States
    {
        IDLE,
        WALKING
    }

    [Export] public int NPCID;
    
    [Export]
    public NavigationAgent3D NavAgent3D {private set; get; }
    [Export]
    public float MoveSpeed {private set; get; } = 3.0f;
    [Export]
    public float RotationSpeed {private set; get; } = 10.0f;

    [Export] private Area3D PlayerInteractArea;
    [Export]
    public NpcController NpcAnimator
    
    {
        get => _npcAnimator;
        private set
        {
            _npcAnimator = value;
            NotifyPropertyListChanged();
        }
    }
    [Export]
    public string CurrentMesh
    {
        get => _currentMesh;
        set
        {
            _currentMesh = value;
            if (NpcAnimator != null)
                NpcAnimator.CurrentMesh = value;
        }
    }

    private NpcController _npcAnimator;
    private string _currentMesh = "";
    private List<Node3D> _wayPoints = new();
    private States _currentState = States.IDLE;
    private Node3D _lastWayPoint;
    private bool _playerInRange = false;

    public override void _ValidateProperty(Godot.Collections.Dictionary property)
    {
        if (property["name"].AsStringName() == PropertyName.CurrentMesh)
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = NpcAnimator?.Skeleton != null
                ? string.Join(",", NpcAnimator.Skeleton.GetChildren().Select(child => child.Name))
                : "";
        }
    }

    public override void _Ready()
    {
        if(Engine.IsEditorHint()) return;

        var wayPointEntities = EntityManager.GetAllOfType((int)Main.EntityTypes.NPCWayPoint);
        foreach (EntityManager.IEntity entity in wayPointEntities)
            _wayPoints.Add((Node3D)entity);

        NpcAnimator.CurrentMesh = _currentMesh;
        PlayerInteractArea.BodyEntered += BodyEntered;
        PlayerInteractArea.BodyExited += BodyExited;
        
        _ = NextState();
    }

    private async Task NextState()
    {
        var states = Enum.GetValues<States>();
        var nextRandomState = states[Random.Shared.Next(states.Length)];

        // Logger.Info($"NextState: {nextRandomState}");

        _currentState = nextRandomState;

        switch (nextRandomState)
        {
            case States.IDLE:
                NpcAnimator.PlayAnimation("Breathing");
                await StartTimer(2.0f);
                _ = NextState();
                return;
            case States.WALKING:
                NpcAnimator.PlayAnimation("Walking");
                SetNextWayPointTarget();
                return;
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if(Engine.IsEditorHint()) return;

        if(_currentState == States.IDLE) return;

        if (NavAgent3D.IsNavigationFinished())
        {
            _ = NextState();
            return;
        }

        var currentLocation = GlobalTransform.Origin;
        var nextLocation = NavAgent3D.GetNextPathPosition();

        var newVelocity = (nextLocation - currentLocation).Normalized() * MoveSpeed;
        Velocity = newVelocity;
        MoveAndSlide();

        var facing = newVelocity with { Y = 0 };
        if (facing.LengthSquared() > 0.0001f)
        {
            var targetBasis = Basis.LookingAt(facing, Vector3.Up);
            GlobalTransform = new Transform3D(GlobalTransform.Basis.Slerp(targetBasis, (float)delta * RotationSpeed), GlobalTransform.Origin);
        }
    }

    private void SetNextWayPointTarget()
    {
        var nextRandomWayPoint = _wayPoints[Random.Shared.Next(_wayPoints.Count)];
        var jitter = new Vector3(Random.Shared.NextSingle() - 0.5f, 0f, Random.Shared.NextSingle() - 0.5f);
        NavAgent3D.TargetPosition = nextRandomWayPoint.GlobalPosition + jitter;

        _lastWayPoint = nextRandomWayPoint;
    }

    public void BodyEntered(Node3D body){
        if (body.IsInGroup("player")) {
            _playerInRange = true;
        }
    }

    public void BodyExited(Node3D body){
        if (body.IsInGroup("player")) {
            _playerInRange = false;
        }
    }

    private async Task StartTimer(float seconds)
    {
        await ToSignal(GetTree().CreateTimer(seconds), SceneTreeTimer.SignalName.Timeout);
    }
}
