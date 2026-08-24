using System.Collections.Generic;
using Godot;
public partial class EntityManager : Node
{
    #region Singelton

    public static EntityManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    public override void _Ready() => Logger.Success("Ready");

    public interface IEntity
    {
        int EntityType { get; }
        void OnRegistered() {}
        void OnUnRegistered() {}
    }

    private ulong _nextEntityID = 0;

    private Dictionary<ulong, IEntity> _entities = [];

    #region Public API

    // Will simply crash if cannot remove id, should handle this better?
    public static ulong RegisterEntity(IEntity entity)
    {
        var id = Instance._nextEntityID;
        Instance._nextEntityID++;

        Instance._entities[id] = entity;
        entity.OnRegistered();

        Logger.Info($"Registered entity {id} of type {entity.EntityType} ({entity.GetType().Name})");
        return id;
    }

    // Will simply crash if cannot remove id, should handle this better?
    public static void UnRegisterEntity(ulong id)
    {
        var entity = Instance._entities[id];

        entity.OnUnRegistered();
        Instance._entities.Remove(id);

        Logger.Info($"Unregistered entity {id} ({entity.GetType().Name})");
    }

    public static bool TryGetFirstOfType(int type, out IEntity entity)
    {
        foreach(IEntity currentEntity in Instance._entities.Values)
        {
            if(currentEntity.EntityType == type)
            {
                entity = currentEntity;
                return true;
            }
        }

        Logger.Warn($"TryGetFirstOfType found no entity of type {type}");
        entity = null;
        return false;
    }

    // Empty if non found of type
    public static List<IEntity> GetAllOfType(int type)
    {
        List<IEntity> entities = [];
        foreach(IEntity currentEntity in Instance._entities.Values)
        {
            if(currentEntity.EntityType == type)
                entities.Add(currentEntity);
        }

        if(entities.Count == 0)
            Logger.Warn($"GetAllOfType found no entities of type {type}");

        return entities;
    }

    #endregion


}