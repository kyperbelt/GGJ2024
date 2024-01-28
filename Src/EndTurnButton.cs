using Godot;
using System;

[Tool]
public partial class EndTurnButton : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	public void hover_on()
	{
		GD.Print("hover_on");
	}

	public void hover_off()
	{
		GD.Print("hover_off");
	}

	public void _on_button_mouse_entered()
	{
		hover_on();
	}

	public void _on_button_mouse_exited()
	{
		hover_off();
	}
}
