using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;


public partial class DialogueManager : Node{
    #region Singleton

    public static DialogueManager Instance {private set; get;}
    public override void _EnterTree() => Instance = this;

    #endregion

    public Array<NPCDialogue> data;
    
    public NPCDialogue ActiveDialogue = null;

    public bool Speaking = false;
    public bool InPrompt = false;
    
    public List<string> tags = new();

    public void LoadDialogueData(Array<NPCDialogue> dialogues){
        for (int i = 0; i < dialogues.Count; i++) {
            dialogues[i].ID = i;
        }
        data = dialogues;
    }

    public void SetActiveDialogue(int d){
        ActiveDialogue = data[d];
    }

    public List<DialogueOption> GetValidOptions(){
        List<DialogueOption> validOptions = new();
        foreach (var option in ActiveDialogue.DialogueOptions) {
            if ((string.IsNullOrEmpty(option.requiredTag) || tags.Contains(option.requiredTag)) && !tags.Contains(option.bannedTag)) {
                validOptions.Add(option);
            }
        }

        return validOptions;
    }

    public void AddTag(string tag){
        if (string.IsNullOrEmpty(tag)) return;
        
        tags.Add(tag);
        Logger.Info($"Added tag: {tag}");
    }
}
