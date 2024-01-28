using Godot;

[GlobalClass]
public partial class Card : MarginContainer
{
    [Signal]
    public delegate void CardReleasedEventHandler(Vector2 position);
    [Export]
    public CardData Data { get; set; }

    private CardVisual Visual { get; set; }
    public CardVisual CardVisual { get => Visual; set => Visual = value; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Visual = GetNode<CardVisual>("MoveWithMouse/CardVisual");
        if (Data != null && Visual != null)
        {
            // GD.Print(Visual);
            Visual.SetCardData(Data);
            Visual.Connect(CardVisual.SignalName.CardReleased, Callable.From<Vector2>((x) => EmitSignal(SignalName.CardReleased, x)));
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }
}
