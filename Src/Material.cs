using Godot;
using System;

[Tool]
public partial class Material : MarginContainer
{
	int materialValue = 0;

	[Export]
	public int MaterialValue
	{
		get { return materialValue; }
		set
		{
			materialValue = value;
			if (_label == null) return;
			_label.Text = value.ToString();
		}
	}

	private Label _label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<Label>("MaterialLabel");
	}
}
