using Godot;

[GlobalClass]
public partial class CardVisual : Control
{
    [Signal]
    public delegate void CardReleasedEventHandler(Vector2 position);
    
    [Export]
    public AnimationPlayer CardAnimator { get; set; }

    [Export]
    public MoveWithMouse MouseMover { get; set; }

    [Export]
    public TextureRect CardBack { get; set; }

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

    [Export]
    public Label HilarityLabel { get; set; }
    
    [Export]
    public HumourIcon HumourIcon { get; set; }

    private CardData _data;

    private string madlibString;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        if (_data != null)
        {
            ReadyUpData(_data);
        }
    }

    public void SetCardData(CardData data)
    {
        _data = data;
        ReadyUpData(_data);
    }

    public void ReadyUpData(CardData newData)
    {
        CardGraphic.Texture = newData.CardIcon;
        if (CardBack != null)
            CardBack.SelfModulate = newData.CardNature.ToColor();
        HumourIcon.SetHumour(newData.CardNature);
        TitleLabel.Text = newData.Name;
        DescriptionLabel.Text = newData.Description;
        EffectLabel.Text = newData.CardType.ToLabelString();
        CostLabel.Text = newData.Cost.ToString();
        HilarityLabel.Text = newData.Hilarity.ToString();

        madlibString = newData.MadlibSentence;
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnMouseHover()
    {
        CardAnimator.Play("card_hover");
        MouseMover.ZIndex = 100;
    }
    public void OnMouseLeave()
    {
        CardAnimator.Play("RESET");
        MouseMover.ZIndex = 0;
    }

    void OnMouseInputEvent(InputEvent ev)
    {

        if (ev is InputEventMouseButton butt)
        {
            if (butt.Pressed)
            {
                MouseMover.IsFollowingMouse = true;
                if (!MouseMover.IsSnapBackSet){
                    MouseMover.SnapBackPosition = MouseMover.GlobalPosition;
                    MouseMover.IsSnapBackSet = true;
                }
            }
            else
            {
                MouseMover.IsFollowingMouse = false;
                EmitSignal(SignalName.CardReleased, MouseMover.GlobalPosition);
            }
        }
    }

    private void ForceTestMadlibs()
    {
        string scenename = GetTree().CurrentScene.Name;

        var _madLibafier = GetTree().Root.GetNodeOrNull<MadLibafier>($"{scenename}/MadLibafier");

        if (_madLibafier == null)
        {
            GD.PrintErr("MadLibafier not found in scene");
            return;
        }

        // test madlibifier 
        string testString = madlibString;
        GD.Print(_madLibafier.GetMadLib(testString));
        GD.Print("-----");
    }
}
