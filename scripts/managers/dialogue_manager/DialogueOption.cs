using Godot;
using System;

[GlobalClass]
public partial class DialogueOption : Resource {
    [Export] public string requiredTag;
    [Export] public string exitTag;
    [Export] public string bannedTag;
    [Export] public string ChoiceText;
    [Export] public string ResponseText;
    [Export] public DialogueOption NextDialogue;

    public int ID;
}