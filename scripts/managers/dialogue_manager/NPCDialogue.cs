using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;


[GlobalClass]
public partial class NPCDialogue : Resource{
    public int ID;

    [Export] public string InitialMessage;

    [Export] public Array<DialogueOption> DialogueOptions;
}
