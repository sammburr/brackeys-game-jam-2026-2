using Godot;
using System;

public partial class DialogueUI : Control{

	[Export] public Label _mainLabel;

	[Export] public Control _promptContainer;
	[Export] public Button _actionButton;

	[Export] public Control MainUI;
	
	
	public override void _Ready(){
		_actionButton.ButtonUp += ActionButtonPressed;
		DialogueManager.Instance.DialogueStarted += SetActiveDialogue;

		MainUI.Visible = false; // do last lol
	}

	public override void _ExitTree(){
		if (DialogueManager.Instance != null)
			DialogueManager.Instance.DialogueStarted -= SetActiveDialogue;
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

	public void SetActiveDialogue(int id){
		GD.Print("aa");
		DialogueManager.Instance.SetActiveDialogue(id);
		LoadDialogueData(DialogueManager.Instance.data[id]);
		MainUI.Visible = true;
		DialogueManager.Instance.Speaking = true;
		ResetUIToPrompts();
	}


	public void HideDialogueScreen(){
		DialogueManager.Instance.InPrompt = false;
		DialogueManager.Instance.Speaking = false;
		DialogueManager.Instance.ExitDialogue();
	}
}
