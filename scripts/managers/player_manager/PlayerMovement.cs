using Godot;

public partial class PlayerMovement : Node3D
{
    [Export] public float Speed = 5.0f;
    [Export] public float JumpVelocity = 4.5f;
    [Export] public float MouseSensitivity = 0.003f;

    [Export] public Node3D CameraPivot;
    [Export] public PlayerManager player;

    private float _gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");

    public override void _Ready(){
        Input.MouseMode = Input.MouseModeEnum.Captured;
    }

    public override void _UnhandledInput(InputEvent @event){
        if (@event is InputEventMouseMotion mouseMotion && Input.MouseMode == Input.MouseModeEnum.Captured) {
            player.RotateY(-mouseMotion.Relative.X * MouseSensitivity);

            CameraPivot.RotateX(-mouseMotion.Relative.Y * MouseSensitivity);
            Vector3 pivotRotation = CameraPivot.Rotation;
            pivotRotation.X = Mathf.Clamp(pivotRotation.X, Mathf.DegToRad(-89f), Mathf.DegToRad(89f));
            CameraPivot.Rotation = pivotRotation;
        }

        if (@event.IsActionPressed("ui_cancel")) {
            Input.MouseMode = Input.MouseMode == Input.MouseModeEnum.Captured
                ? Input.MouseModeEnum.Visible
                : Input.MouseModeEnum.Captured;
        }
    }

    public override void _PhysicsProcess(double delta){
        Vector3 velocity = player.Velocity;

        if (!player.IsOnFloor())
            velocity.Y -= _gravity * (float)delta;

        Vector2 inputDir = PlayerManager.Instance.InputVector;
        Vector3 direction = (player.Transform.Basis * new Vector3(inputDir.X, 0, inputDir.Y)).Normalized();

        if (direction.LengthSquared() > 0) {
            velocity.X = direction.X * Speed;
            velocity.Z = direction.Z * Speed;
        } else {
            velocity.X = Mathf.MoveToward(velocity.X, 0, Speed);
            velocity.Z = Mathf.MoveToward(velocity.Z, 0, Speed);
        }

        player.Velocity = velocity;
        player.MoveAndSlide();

    }
}