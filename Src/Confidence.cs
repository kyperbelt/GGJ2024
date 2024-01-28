using Godot;
using System;

[Tool]
public partial class Confidence : MarginContainer
{
    int maxHealth = 0;
    Color playerColor;

    [Export]
    public int MaxHealth
    {
        get { return maxHealth; }
        set
        {
            maxHealth = value;
            if (_label == null || _progressBar == null)
            {
                return;
            }
            _progressBar.MaxValue = maxHealth;
            _label.Text = _currentHealth + "/" + maxHealth;
        }
    }

    [Export]
    public Color PlayerColor
    {
        get { return playerColor; }
        set
        {
            playerColor = value;
            if (_progressBar == null)
            {
                return;
            }
            _progressBar.TintProgress = playerColor;
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
	    _progressBar.MaxValue = maxHealth;
	    _label.Text = _currentHealth + "/" + maxHealth;
    }

    public void SetCurrentHealth(int health)
    {
        _currentHealth = health;
        if (_label == null || _progressBar == null)
        {
            return;
        }
        _label.Text = $"{health}/{maxHealth}";
	_progressBar.Value = health;
    }
}
