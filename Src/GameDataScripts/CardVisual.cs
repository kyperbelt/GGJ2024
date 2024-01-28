using Godot;

[GlobalClass]
public partial class CardVisual : Control
{
    [Export]
    public AnimationPlayer CardAnimator { get; set; }

    [Export]
    public MoveWithMouse MouseMover { get; set; }

    [Export]
    public TextureRect CardGraphic { get; set; }

    [Export]
    public Label TitleLabel { get; set; }

    [Export]
    public Label DescriptionLabel { get; set; }

    [Export]
    public Label EffectLabel { get; set; }

    [Export]
    public Label CostLabel { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {

    }

    public void ReadyUpData(CardData newData)
    {
        CardGraphic.Texture = newData.CardIcon;

        TitleLabel.Text = newData.Name;
        DescriptionLabel.Text = newData.Description;
        EffectLabel.Text = newData.CardNature.ToLabelString();
        CostLabel.Text = newData.Cost.ToString();
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnMouseHover()
    {
        CardAnimator.Play("card_hover");
    }
    public void OnMouseLeave()
    {
        CardAnimator.Play("RESET");
    }

    void OnMouseInputEvent(InputEvent ev)
    {

        if (ev is InputEventMouseButton butt)
        {
            if (butt.Pressed)
            {
                GD.Print("Mouse Clicked");
                MouseMover.IsFollowingMouse = true;
            }
            else
            {
                GD.Print("Mouse Released");
                MouseMover.IsFollowingMouse = false;
            }
        }
    }
}
