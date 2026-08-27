using Godot;

public partial class PlayerManager : CharacterBody3D
{
    #region Singleton

    public static PlayerManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    #region Input Classes
    private class PlayerInput : ICustomParsedInput
    {
        public Vector2 MoveDirection;
        public bool PlayerInteract;
    }
    private class PlayerInputContext : ICustomInputContext
    {
        private Vector2 _moveDirection;
        private bool interact;

        public ICustomParsedInput Restart()
        {
            _moveDirection = Vector2.Zero;
            
            return new PlayerInput { MoveDirection = _moveDirection, PlayerInteract = false };
        }

        public ICustomParsedInput TransformInput(InputEvent @event)
        {
            bool interact = Input.IsActionPressed("player_interact");

            float x = (Input.IsActionPressed("player_move_right") ? 1f : 0f) - (Input.IsActionPressed("player_move_left") ? 1f : 0f);
            float y = (Input.IsActionPressed("player_move_back") ? 1f : 0f) - (Input.IsActionPressed("player_move_forward") ? 1f : 0f);
            var newMoveDirection = new Vector2(x, y).Normalized();

            // if (newMoveDirection == _moveDirection && !interact) return new EmptyParsedInput();

            _moveDirection = newMoveDirection;
            
            return new PlayerInput {
                MoveDirection = _moveDirection,
                PlayerInteract = interact
            };
        }
    }
    #endregion


    private PlayerInputContext _inputContext;
    public Vector2 InputVector;
    public bool PlayerInteracted;
    

    public override void _Ready(){
        _inputContext = new PlayerInputContext();
        InputManager.PushContext(_inputContext);
        InputManager.InputParsed += OnInputParsed;
    }
    
    public override void _ExitTree(){
        InputManager.InputParsed -= OnInputParsed;
    }
    
    private void OnInputParsed(ICustomParsedInput parsed){
        if (parsed is not PlayerInput input) return;
        InputVector = input.MoveDirection;
        PlayerInteracted = input.PlayerInteract;
    }
    
}
