using Godot;
using System;

public partial class HandArea : HBoxContainer
{
    [Signal]
    public delegate void CardPlayedEventHandler(int index, Vector2 position);

    [Export]
    public PackedScene CardPrefab;

    [Export]
    public int MinValue { get; set; } = -100;

    [Export]
    public int MaxValue { get; set; } = 20;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        Connect(SignalName.ChildEnteredTree, Callable.From<Node>((x) =>
        {
            Card card = x as Card;
            card.Connect(Card.SignalName.CardReleased, Callable.From<Vector2>(
                (x) =>
                {
                    EmitSignal(SignalName.CardPlayed, GetChildren().IndexOf(card), x);
                }));
            AddThemeConstantOverride("separation", Math.Clamp((MaxValue - GetChildCount() * GetChildCount() * 2), MinValue, MaxValue));
        }));

	Connect(SignalName.ChildExitingTree, Callable.From<Node>((x) =>
	{
	    AddThemeConstantOverride("separation", Math.Clamp((MaxValue - GetChildCount() * GetChildCount() * 2), MinValue, MaxValue));
	}));
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }


    public void DrawCard(CardData cardData)
    {
        Card cardInstance = CardPrefab.Instantiate<Card>();
        cardInstance.Data = cardData;

        AddChild(cardInstance);

    }

    public void DiscardCard(int index)
    {
        Card card = GetChildren()[index] as Card;
        GetChildren().RemoveAt(index);
        card.Data = null;
        card.QueueFree();
    }

}
