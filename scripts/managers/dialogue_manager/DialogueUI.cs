using Godot;
using System;

public partial class DialogueUI : Control{

	[Export] private Label _mainLabel;

	[Export] private Control _promptContainer;
	[Export] private Button _okButton;
	
	
	private NPCDialogue _activeDialogue;
	private bool _justPrompted;
	
	
	public override void _Ready(){
		LoadDialogueData(DialogueManager.Instance.data);
		_okButton.ButtonUp += ResetToPrompts;
	}

	private void LoadDialogueData(NPCDialogue d){
		_activeDialogue = d;
		_mainLabel.Text = d.InitialMessage;

		foreach (Node child in _promptContainer.GetChildren())
			child.QueueFree();


		for (int i = 0; i < d.DialogueOptions.Count; i++) {
			var option = d.DialogueOptions[i];
			var button = new Button();
			button.Text = option.ChoiceText;
			_promptContainer.AddChild(button);

			int index = i; // annoying
			button.ButtonUp += () => SelectPrompt(index);
		}
	}

	private void SelectPrompt(int p){
		_mainLabel.Text = _activeDialogue.DialogueOptions[p].ResponseText;
		_promptContainer.Visible = false;
	}

	private void ResetToPrompts(){
		_promptContainer.Visible = true;
		_mainLabel.Text = _activeDialogue.InitialMessage;
	}
}
