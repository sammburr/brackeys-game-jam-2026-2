using Godot;
using System;

public partial class PlayerManager : CharacterBody3D
{
    
    #region Singleton

    public static PlayerManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion
    
    private class PlayerInput : ICustomParsedInput
    {
        public bool PlayerMoveLeft;
        public bool PlayerMoveRight;
        public bool PlayerMoveForward;
        public bool PlayerMoveBack;
        public bool PlayerInteract;
    }
    private class PlayerInputContext : ICustomInputContext
    {
        public ICustomParsedInput Restart()
        {
            return new PlayerInput {
                PlayerMoveLeft = false,
                PlayerMoveRight = false,
                PlayerMoveForward = false, 
                PlayerMoveBack = false
            };
        }

        public ICustomParsedInput TransformInput(InputEvent @event)
        {
            return new PlayerInput{PlayerMoveLeft = @event.IsActionPressed("player_move_left"),
                PlayerMoveRight = @event.IsActionPressed("player_move_right"),
                PlayerMoveForward = @event.IsActionPressed("player_move_forward"), 
                PlayerMoveBack = @event.IsActionPressed("player_move_back"),
                PlayerInteract = @event.IsActionPressed("player_interact")};
        }
    }

    private PlayerInputContext _inputContext;
    public Vector2 InputVector;
    
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
        
        float x = (input.PlayerMoveRight ? 1f : 0f) - (input.PlayerMoveLeft ? 1f : 0f);
        float y = (input.PlayerMoveBack ? 1f : 0f) - (input.PlayerMoveForward ? 1f : 0f);

        
        InputVector = new Vector2(x, y).Normalized();
        Logger.Info($"{x} {y}, {InputVector}");
    }
    
}
