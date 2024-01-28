using Godot;

[GlobalClass]
public partial class Card : Control
{
	[Export]
	public CardData Data { get; set; }

	private CardVisual Visual { get; set; }

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
	Visual = GetNode<CardVisual>("MoveWithMouse/CardVisual");
        if (Data != null && Visual != null)
        {
			GD.Print(Visual);
            Visual.SetCardData(Data);
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
