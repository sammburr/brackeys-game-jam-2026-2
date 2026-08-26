using System;
using System.Collections.Generic;
using Godot;

public partial class InputManager : Node
{
    #region Singelton

    public static InputManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion


    #region Debug

    public override void _Ready() => Logger.Success("Ready");

    #endregion
    
    // Raw C# event to send a non-Godot object.
    public static event Action<ICustomParsedInput> InputParsed;

    public Stack<ICustomInputContext> InputStack = new();

    public override void _Input(InputEvent @event)
    {
        ICustomInputContext context;
        if(InputStack.TryPeek(out context))
        {
            ICustomParsedInput parsedInput = context.TransformInput(@event);
            if(parsedInput is not EmptyParsedInput)
            {
                Logger.Info($"Parsed {parsedInput.GetType().Name} via {context.GetType().Name}");
                InputParsed?.Invoke(parsedInput);
            }
        }
        else if(@event.IsPressed() && !@event.IsEcho())
        {
            // Logger.Warn($"Received {@event.GetType().Name} but InputStack is empty, ignoring");
        }
    }


    #region Public API

    public static void PushContext(ICustomInputContext context)
    {
        if(Instance.InputStack.TryPeek(out var previousTop))
        {
            EmitRestart(previousTop);
        }

        Instance.InputStack.Push(context);
        Logger.Info($"Pushed context: {context.GetType().Name} (depth {Instance.InputStack.Count})");
    }

    // Will do nothing if there is nothing to Pop, possibly a better way to handle.
    public static void PopContext()
    {
        if(!Instance.InputStack.TryPop(out var popped))
        {
            Logger.Error("Tried to PopContext but InputStack was already empty");
            return;
        }

        Logger.Info($"Popped context: {popped.GetType().Name} (depth {Instance.InputStack.Count})");

        if(Instance.InputStack.TryPeek(out var context))
        {
            EmitRestart(context);
            Logger.Info($"Restarted context: {context.GetType().Name}");
        }
        else
        {
            Logger.Warn("InputStack is now empty after pop");
        }
    }

    #endregion

    private static void EmitRestart(ICustomInputContext context)
    {
        var resetInput = context.Restart();
        if(resetInput is not EmptyParsedInput)
            InputParsed?.Invoke(resetInput);
    }
}