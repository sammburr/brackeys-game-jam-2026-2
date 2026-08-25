using Godot;
using System;

[GlobalClass]
public partial class DialogueOption : Resource {
    [Export] public string ChoiceText;
    [Export] public string ResponseText;
}