using Godot;

public interface ICustomInputContext
{
    public ICustomParsedInput Restart();
    public ICustomParsedInput TransformInput(InputEvent @event);
}

public class EmptyParsedInput : ICustomParsedInput {}