using Godot;
using System;

public partial class DialogueUI : Control{

	[Export] public Label _mainLabel;

	[Export] public Control _promptContainer;
	[Export] public Button _actionButton;

	[Export] public Control MainUI;
	
	
	public override void _Ready(){
		_actionButton.ButtonUp += ActionButtonPressed;
		MainUI.Visible = false;

	}

	private void LoadDialogueData(NPCDialogue d){
		_mainLabel.Text = d.InitialMessage;

		foreach (Node child in _promptContainer.GetChildren())
			child.QueueFree();

		var options = DialogueManager.Instance.GetValidOptions();
			
		
		foreach (var option in options) {
			var button = new Button();
			button.Text = option.ChoiceText;
			_promptContainer.AddChild(button);
			button.ButtonUp += () => SelectedPrompt(option);
		}
	}

	private void SelectedPrompt(DialogueOption option){
		_mainLabel.Text = option.ResponseText;
		_promptContainer.Visible = false;
		DialogueManager.Instance.InPrompt = true;
		DialogueManager.Instance.CurrentDialogue = option;
		DialogueManager.Instance.AddTag(option.exitTag);
		DialogueManager.Instance.MarkAsSaid(option.ID);
		_actionButton.Text = "Continue";
	}

	private void ActionButtonPressed(){
		if (DialogueManager.Instance.InPrompt) {
			if (DialogueManager.Instance.CurrentDialogue.NextDialogue != null) {
				SelectedPrompt(DialogueManager.Instance.CurrentDialogue.NextDialogue);
			}
			else {
				ResetUIToPrompts();

			}
		}
		else {
			HideDialogueScreen();
		}
	}
	
	private void ResetUIToPrompts(){
		_promptContainer.Visible = true;
		DialogueManager.Instance.InPrompt = false;
		LoadDialogueData(DialogueManager.Instance.DialogueActive);
		_actionButton.Text = "Leave";
	}

	public void SetActiveDialogue(int d){
		DialogueManager.Instance.SetActiveDialogue(d);
		LoadDialogueData(DialogueManager.Instance.data[d]);
		MainUI.Visible = true;
		DialogueManager.Instance.Speaking = true;
		ResetUIToPrompts();
	}

	public void HideDialogueScreen(){
		MainUI.Visible = false;
		DialogueManager.Instance.InPrompt = false;
		DialogueManager.Instance.Speaking = false;
	}
}
