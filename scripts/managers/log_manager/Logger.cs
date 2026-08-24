using System;
using System.IO;
using System.Runtime.CompilerServices;
using Godot;

public partial class Logger : Node
{
    #region Singelton

    public static Logger Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    public override void _Ready() => Success("Ready");

    private enum Level { Info, Success, Warn, Error }

    #region Public API

    public static void Info(string message, [CallerFilePath] string callerPath = "")
        => Emit(Level.Info, message, callerPath);

    public static void Success(string message, [CallerFilePath] string callerPath = "")
        => Emit(Level.Success, message, callerPath);

    public static void Warn(string message, [CallerFilePath] string callerPath = "")
        => Emit(Level.Warn, message, callerPath);

    public static void Error(string message, [CallerFilePath] string callerPath = "")
        => Emit(Level.Error, message, callerPath);

    #endregion

    // [CallerFilePath] fills in the caller's script path automatically, so nobody
    // has to pass their own name in - "what called it" is free.
    private static void Emit(Level level, string message, string callerPath)
    {
        var timestamp = DateTime.Now.ToString("HH:mm:ss");
        var source = Path.GetFileNameWithoutExtension(callerPath);

        var (color, bodyColor, tag) = level switch
        {
            Level.Success => ("#7CFC93", null, "OK"),
            Level.Warn => ("#FFD166", "#E0AD6E", "WARN"),
            Level.Error => ("#FF6B6B", "#E08585", "ERR"),
            _ => ("#6FC3DF", null, "INFO"),
        };

        var body = bodyColor is null ? message : $"[color={bodyColor}]{message}[/color]";
        if (level == Level.Error)
            body = $"[shake rate=20.0 level=6]{body}[/shake]";

        GD.PrintRich($"[color=#5A5A5A]{timestamp}[/color] [color={color}][{tag}][/color] [color=#5A5A5A]{source}[/color] :: {body}");
    }
}
