using Godot;
using System;
using System.Linq;
using Godot.Collections;

public partial class NPCManager : Node3D{


	public Array<NpcEntity> NPCs;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready(){
		NPCs = new Array<NpcEntity>(GetChildren().OfType<NpcEntity>());
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta){
		
		bool interacted = PlayerManager.Instance.PlayerInteracted;
		if (interacted) {
			NpcEntity closestNPC = null;
			float npcDistance = 99999f;
			foreach (var NPC in NPCs) {
				if (NPC.PlayerInRange) {
					float dist = (NPC.Position - PlayerManager.Instance.Position).Length();
					if (npcDistance > dist) {
						npcDistance = dist;
						closestNPC = NPC;
					}
				}
			}

			if (closestNPC != null) {
				GD.Print(closestNPC.ID);
				DialogueManager.Instance.StartDialogue(closestNPC.ID);
			}
		}
	}
}
