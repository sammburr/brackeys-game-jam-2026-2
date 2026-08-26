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
    
    public NPCDialogue DialogueActive = null;
    
    
    public bool Speaking = false;
    public bool InPrompt = false;
    public DialogueOption CurrentDialogue;
    
    public List<string> tags = new();
    public List<int> spokenOptionIDs = new();

    public void LoadDialogueData(Array<NPCDialogue> dialogues){
        int dialogueCount = 0;
        for (int i = 0; i < dialogues.Count; i++) {
            dialogues[i].ID = i;
            for (int j = 0; j < dialogues[i].DialogueOptions.Count; j++) {
                dialogues[i].DialogueOptions[j].ID = dialogueCount;
                dialogueCount++;
            }
        }
        data = dialogues;
    }

    public void SetActiveDialogue(int id){
        DialogueActive = data[id];
    }

    public List<DialogueOption> GetValidOptions()
    {
        List<DialogueOption> validOptions = new();
        foreach (var option in DialogueActive.DialogueOptions)
        {
            bool banned = tags.Contains(option.bannedTag);
            bool alreadySaid = HasBeenSaid(option.ID);
            bool meetsRequirement = string.IsNullOrEmpty(option.requiredTag) || tags.Contains(option.requiredTag);

            if (!banned && !alreadySaid && meetsRequirement)
                validOptions.Add(option);
        }

        return validOptions;
    }

    public bool HasBeenSaid(int id){
        return spokenOptionIDs.Contains(id);
    }

    public void MarkAsSaid(int id){
        spokenOptionIDs.Add(id);
    }

    public DialogueOption GetOptionFromID(int id){
        foreach (var dialogue in data) {
            foreach (var option in dialogue.DialogueOptions) {
                if (option.ID == id) return option;
            }
        }

        return null;
    }
    
    public void AddTag(string tag){
        if (string.IsNullOrEmpty(tag)) return;
        
        tags.Add(tag);
        Logger.Info($"Added tag: {tag}");
    }
}
