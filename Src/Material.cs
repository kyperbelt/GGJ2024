using Godot;
using System;

[Tool]
public partial class Material : MarginContainer
{
	
	private int _maxMaterialValue = 5;
	[Export]
	public int MaxMaterialValue
	{
		get { return _maxMaterialValue; }
		set
		{
			_maxMaterialValue = value;
			if (_label == null) return;
			_label.Text = $"{MaterialValue}/{value}";
		}
	}

	private int materialValue = 0;

	[Export]
	public int MaterialValue
	{
		get { return materialValue; }
		set
		{
			// if (value > _maxMaterialValue) return; 
			if (value < 0) return;
			materialValue = value;
			if (_label == null) return;
			_label.Text = $"{MaterialValue}/{MaxMaterialValue}";
		}
	}

	private Label _label;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_label = GetNode<Label>("MaterialLabel");
	}
}
