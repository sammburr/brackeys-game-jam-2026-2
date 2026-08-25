using Godot;
using System;
using Godot.Collections;


public partial class DialogueManager : Node{
    #region Singleton

    public static DialogueManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    public NPCDialogue data;

    public void LoadDialogueData(Array<NPCDialogue> dialogues){
        data = dialogues[0];
    }
    
    

}
