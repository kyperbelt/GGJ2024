using Godot;
using System;

[Tool]
public partial class DiscardButton : Control
{
    int discardPileNumber = 0;

    [Export]
    public int DiscardPileNumber
    {
        get { return discardPileNumber; }
        set
        {
            discardPileNumber = value;
            if (_label == null) return;
            _label.Text = value.ToString();
        }
    }

    private Label _label;

    public override void _Ready()
    {
        _label = GetNode<Label>("DiscardPileLabel");
        _label.Text = discardPileNumber.ToString();
    }
}
