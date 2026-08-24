using System.Collections.Generic;
using System.Xml;
using Godot;

public partial class SceneManager : Node
{
    #region Singelton

    public static SceneManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    public Dictionary<string, PackedScene> RegisteredScenes {private set; get;} = new();
    public HashSet<Node> SceneInstances {private set; get;} = new();

    #region Public API

    #region Debug

    public override void _Ready() => Logger.Success("Ready");

    #endregion

    // Calls GD.Load<T>, therefore blocks, best call this for all scenes at first loadup.
    // May otherwise cause stuttering during gameplay?
    public static void RegisterScene(string name, string path)
    {
        if(!ResourceLoader.Exists(path, nameof(PackedScene)))
        {
            Logger.Error($"Could not register scene of path: {path}, panic!");
            throw new($"Could not register scene of path: {path}, panic!");
        }

        var scene = GD.Load(path) as PackedScene;
        Instance.RegisteredScenes.Add(name, scene);
        Logger.Info($"Registered scene '{name}' from {path}");
    }

    public static void RegisterScenes(Dictionary<string, string> scenes)
    {
        foreach((string id, string path) in scenes)
        {
            RegisterScene(id, path);
        }
    }

    // Where 'name' is the name used to register the scene via `RegisterScene(name, path)`
    public static bool TryInstanciateScene(string name, out Node instance)
    {
        if (!Instance.RegisteredScenes.ContainsKey(name))
        {
            Logger.Warn($"TryInstanciateScene could not find registered scene '{name}'");
            instance = null;
            return false;
        }

        var scene = Instance.RegisteredScenes[name];
        instance = scene.Instantiate();

        // Explicitly track as is safer than just listing children?
        Instance.SceneInstances.Add(instance);

        Instance.AddChild(instance);
        Logger.Success($"Instanciated scene '{name}'");
        return true;
    }

    // Does nothing if could not find instance
    public static void TryFreeScene(Node instance)
    {
        if(!Instance.SceneInstances.Remove(instance))
            Logger.Warn($"TryFreeScene could not find tracked instance {instance.Name}");
        else
            Logger.Info($"Freed scene instance {instance.Name}");

        instance.QueueFree();
    }

    public static void Clear()
    {
        Logger.Info($"Freed all scenes instances");

        foreach(Node scene in Instance.SceneInstances)
            scene.QueueFree();

        Instance.SceneInstances.Clear();
    }

    #endregion

}
