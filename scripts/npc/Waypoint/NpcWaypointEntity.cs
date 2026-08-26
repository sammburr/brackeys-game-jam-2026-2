using Godot;
using System;

public partial class NpcWaypointEntity : Node3D, EntityManager.IEntity
{
    public int EntityType => (int)Main.EntityTypes.NPCWayPoint;
    private ulong _id;

    public override void _EnterTree()
    {
        _id = EntityManager.RegisterEntity(this);
    }

    public override void _ExitTree()
    {
        EntityManager.UnRegisterEntity(_id);
    }
}
