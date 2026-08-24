using System.Linq;
using Godot;
using Godot.Collections;

[Tool]
public partial class Main : Node
{
    [Export]
    public Dictionary<string, string> Scenes
    {
        get => _scenes;
        set
        {
            _scenes = value;
            NotifyPropertyListChanged();
        }
    }

    [Export]
    public string StartupScene { get; private set; } = "";

    [Export]
    public Dictionary<string, string> AudioStreams {get; private set;} = new();

    private Dictionary<string, string> _scenes = new();

    // This is just boiler plate for making StartupScene a drop down of options populated
    // from the Scenes dictionary.
    public override void _ValidateProperty(Dictionary property)
    {
        if (property["name"].AsStringName() == PropertyName.StartupScene)
        {
            property["hint"] = (int)PropertyHint.Enum;
            property["hint_string"] = _scenes.Count > 0 ? string.Join(",", _scenes.Keys) : "";
        }
    }


    public override void _Ready()
    {
        SceneManager.RegisterScenes(Scenes.ToDictionary());
        SoundManager.RegisterStreams(AudioStreams.ToDictionary());

        SceneManager.TryInstanciateScene(StartupScene, out _);
    }
}
