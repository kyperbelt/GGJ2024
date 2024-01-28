using Godot;
using System;

public partial class SpeechBubble : Node2D
{
	[Export]
	private Node2D _bubbleSprite;
	
	[Export]
	private Label _speechLabel;

	[Export]
	private MadLibafier _madLibafier;
	
	[Export]
	private Timer _bubbleHideTimer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		SetBubbleAndTextVisible(false);
	}

	public void ShowDialogueLine(string madLibString)
	{
		SetBubbleAndTextVisible(true);
		_speechLabel.Text = _madLibafier.GetMadLib(madLibString);
		_bubbleHideTimer.Stop();
		_bubbleHideTimer.Start();
	}

	private void SetBubbleAndTextVisible(bool visible)
	{
		_bubbleSprite.Visible = visible;
		_speechLabel.Visible = visible;
	}

	private void OnBubbleHideTimer()
	{
		SetBubbleAndTextVisible(false);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
