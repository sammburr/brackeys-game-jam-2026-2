using Godot;
using System;
using System.Collections.Generic;
using Godot.Collections;


[GlobalClass]
public partial class NPCDialogue : Resource{
    [Export] public string InitialMessage;

    [Export] public Array<DialogueOption> DialogueOptions;
}
