using Godot;
using System;

[GlobalClass]
public partial class HandVisual : Control
{
	[Export]
	public Card CardTemplate { get; set; }

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
