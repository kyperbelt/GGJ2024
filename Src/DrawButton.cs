using Godot;
using System;

[Tool]
public partial class DrawButton : Control
{
	int drawPileNumber = 0;

	[Export]
	public int DrawPileNumber
	{
		get { return drawPileNumber; }
		set
		{
			drawPileNumber = value;
			if (_label == null) return;
			_label.Text = value.ToString();
		}
	}

	private Label _label;

	public override void _Ready()
	{
		_label = GetNode<Label>("DrawPileLabel");
	}
}
