using Godot;
using System;

public partial class MainMenu : Node
{

    [Export]
    public Button StartGameButton {private set; get;}

    [Export] private string _nextScene;
    [Export]
    public Button QuitGameButton {private set; get;}

    private class MainMenuInput : ICustomParsedInput
    {
        public bool PressedQuit;
    }
    private class MainMenuInputContext : ICustomInputContext
    {
        public ICustomParsedInput Restart()
        {
            return new MainMenuInput{PressedQuit = false};
        }

        public ICustomParsedInput TransformInput(InputEvent @event)
        {
            var pressedQuit = @event.IsActionPressed("main_menu_quit_game");

            if(!pressedQuit) return new EmptyParsedInput();

            return new MainMenuInput{PressedQuit = pressedQuit};
        }
    }

    public override void _Ready()
    {
        InputManager.PushContext(new MainMenuInputContext());

        SoundManager.PlaySound("menu-ambi", true);
        StartGameButton.Pressed += StartGame;
        QuitGameButton.Pressed += QuitGame;

        InputManager.InputParsed += OnInputParsed;
    }

    private void StartGame()
    {
        InputManager.PopContext();

        SoundManager.StopAllSounds();
        SceneManager.Clear(); // this frees this script, but that is okay as the scene tree will still run the rest of this frame!
        SceneManager.TryInstanciateScene(_nextScene, out _);
    }

    private void QuitGame()
    {
        GetTree().Quit();
    }

    private void OnInputParsed(ICustomParsedInput parsed)
    {
        if(parsed is not MainMenuInput input) return;

        if(input.PressedQuit) QuitGame();
    }

}
