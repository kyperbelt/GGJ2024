using Godot;
using System;

public partial class HandArea : HBoxContainer
{
    [Signal] 
    public delegate void CardDrawnAnimationFinishedEventHandler();
    [Signal] 
    public delegate void CardDiscardAnimationFinishedEventHandler();

    [Signal]
    public delegate void CardPlayedEventHandler(int index, Vector2 position);

    [Export]
    public PackedScene CardPrefab;

    [Export]
    public int MinValue { get; set; } = -100;

    [Export]
    public int MaxValue { get; set; } = 20;

    [Export]
    public DrawButton DrawButton { get; set; }
    [Export] 
    public DiscardButton DiscardButton { get; set; }
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


    public async void DrawCard(CardData cardData)
    {
        Card cardInstance = CardPrefab.Instantiate<Card>();
        Card cardInstanceDiplicate = CardPrefab.Instantiate<Card>();

        cardInstance.Data = cardData;
        cardInstanceDiplicate.Data = cardData;
        cardInstance.Modulate = new Color(1, 1, 1, 0.0f);

        GD.Print("card drawn");
        AddChild(cardInstance);
        var timer = GetTree().CreateTimer(.2f);
        await ToSignal(timer, "timeout");
        // await ToSignal(cardInstance, "ready");

        Tween tween = GetTree().CreateTween();
        GetParent().AddChild(cardInstanceDiplicate);
        cardInstanceDiplicate.GlobalPosition = DrawButton.GlobalPosition;
        tween.TweenProperty(cardInstanceDiplicate, "global_position", cardInstance.GlobalPosition, .2);

        await ToSignal(tween, "finished");
        cardInstanceDiplicate.QueueFree();
        EmitSignal(SignalName.CardDrawnAnimationFinished);
        cardInstance.Modulate = new Color(1, 1, 1, 1.0f);


    }

    public async void DiscardCard(int index)
    {
        
        GD.Print("card discarded " + index + " ChildCount " + GetChildren().Count);
        Card card = GetChildren()[index] as Card;
        card.Modulate = new Color(1, 1, 1, 0.0f);
        Card cardInstance = CardPrefab.Instantiate<Card>();
        cardInstance.Data = card.Data;
        cardInstance.GlobalPosition = card.CardVisual.MouseMover.GlobalPosition;
        GetParent().AddChild(cardInstance);

        card.Data = null;
        card.QueueFree();
        await ToSignal(card, Node.SignalName.TreeExiting);
        

        Tween tween = GetTree().CreateTween();
        tween.TweenProperty(cardInstance, "global_position", DiscardButton.GlobalPosition, .2);
        await ToSignal(tween, "finished");

        cardInstance.QueueFree();

        EmitSignal(SignalName.CardDiscardAnimationFinished);
    }

}
