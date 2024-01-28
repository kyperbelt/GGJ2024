using Godot;
using System;

[Tool]
public partial class Confidence : MarginContainer
{
	int maxHealth = 0;

	[Export]
	public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
			maxHealth = value;
			if (_label == null || _progressBar == null) {
				return;
			}
			_progressBar.MaxValue = maxHealth;
            _label.Text = _currentHealth + "/" + maxHealth;
        }
    }

	private Label _label;
	private TextureProgressBar _progressBar;
	private int _currentHealth = 0;


	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{	
		_label = GetNode<Label>("TextLabel");
		_progressBar = GetNode<TextureProgressBar>("Confidence");
	}

	public void SetCurrentHealth(int health)
	{
		_currentHealth = health;
		_label.Text = $"{health}/{maxHealth}";
	}
}
