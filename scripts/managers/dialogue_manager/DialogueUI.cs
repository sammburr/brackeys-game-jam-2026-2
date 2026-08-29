using Godot;
using System;

public partial class DialogueUI : Control{

	[Export] private RichTextLabel _mainLabel;

	[Export] private Control _promptContainer;
	[Export] private Button _actionButton;

	[Export] private PackedScene _dialogueButtonTemplate;

	[Export] private Control MainUI;

	[Export] private string _mainTextPrefix;
	
	[Export] private float _scrollSpeed = 60f; // scroll speed

	private Tween _textTween;
	private bool _isScrolling;

	public override void _Ready(){
		_actionButton.ButtonUp += ActionButtonPressed;
		DialogueManager.Instance.DialogueStarted += SetActiveDialogue;

		_mainLabel.VisibleRatio = 1f;
		MainUI.Visible = false; // do last lol
	}

	public override void _ExitTree(){
		if (DialogueManager.Instance != null)
			DialogueManager.Instance.DialogueStarted -= SetActiveDialogue;
	}

	
	private void LoadDialogueData(NPCDialogue d){
		SetMainText(_mainTextPrefix + d.InitialMessage);

		foreach (Node child in _promptContainer.GetChildren())
			child.QueueFree();

		var options = DialogueManager.Instance.GetValidOptions();
			
		
		foreach (var option in options) {
			var button = (Button) _dialogueButtonTemplate.Instantiate();
			button.Text = option.ChoiceText;
			_promptContainer.AddChild(button);
			button.ButtonUp += () => PromptSelected(option);
		}
	}

	private void PromptSelected(DialogueOption option){
		SetMainText(_mainTextPrefix + option.ResponseText);
		_promptContainer.Visible = false;
		DialogueManager.Instance.InPrompt = true;
		DialogueManager.Instance.CurrentDialogue = option;
		DialogueManager.Instance.AddTag(option.exitTag);
		DialogueManager.Instance.MarkAsSaid(option.ID);
		_actionButton.Text = "Continue";
	}

	private void SetMainText(string t){
		_mainLabel.Text = t;
		_mainLabel.VisibleRatio = 0f;

		_textTween?.Kill();
		_isScrolling = true;

		float duration = t.Length / _scrollSpeed;
		_textTween = CreateTween();
		_textTween.TweenProperty(_mainLabel, "visible_ratio", 1f, duration);
		_textTween.Finished += () => _isScrolling = false;
	}
	
	private void ActionButtonPressed(){
		if (_isScrolling){
			_textTween?.Kill();
			_mainLabel.VisibleRatio = 1f;
			_isScrolling = false;
			return;
		}
		
		if (DialogueManager.Instance.InPrompt) {
			if (DialogueManager.Instance.CurrentDialogue.NextDialogue != null) {
				PromptSelected(DialogueManager.Instance.CurrentDialogue.NextDialogue);
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
