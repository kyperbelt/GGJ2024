using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

public partial class Battle : Node2D
{
    // private MadLibafier _madLibafier;
    [Export]
    private SpeechBubble _playerSpeechBubble;
    
    [Export]
    private SpeechBubble _hecklerSpeechBubble;

    [Export]
    private CardData[] _cardPrototypes;

    private List<CardData> _deck = new();
    private List<CardData> _hand = new();

    [Export]
    private int _deckSize = 64;

    private enum TurnType
    {
	    Player,
	    Heckler
    }
    private TurnType _turnType = TurnType.Player;
    
    public override void _Ready()
    {
        /*
        _madLibafier = GetNode<MadLibafier>("MadLibafier");

        if (_madLibafier == null)
        {
            GD.PushError("MadLibafier not found in Battle scene");
        }

        // test madlibifier 
        string testString = "The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].";
        GD.Print(_madLibafier.GetMadLib(testString));
        */
        
        // Create randomized deck
        for (int i = 0; i < _deckSize; ++i)
        {
			var randomCardProtoIndex = Random.Shared.Next(0, _cardPrototypes.Length);
			var randomCardProto = _cardPrototypes[randomCardProtoIndex];
			_deck.Add(randomCardProto);
        }
		PrintDeck();
    }
    
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (!eventKey.Pressed) return;
			switch (eventKey.Keycode)
			{
				case Key.Space:
				{
					// Temp logic to advance turn and show new dialogue
					var targetBubble = _turnType switch
					{
						TurnType.Player => _playerSpeechBubble,
						TurnType.Heckler => _hecklerSpeechBubble,
						_ => throw new InvalidOperationException($"Unknown turn type: {_turnType}"),
					};
					targetBubble.ShowDialogueLine("The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].");
					_turnType = _turnType == TurnType.Player ? TurnType.Heckler : TurnType.Player;
					break;
				}
				case Key.D:
				{
					// Draw card
					if (_deck.Count == 0)
					{
						GD.Print("Deck is Empty!");
					}
					else
					{
						var nextCard = _deck[0];
						_deck.RemoveAt(0);
						GD.Print($"\ud83d\ude80 Draw Card: {nextCard}");
						_hand.Add(nextCard);
						PrintDeck();
						PrintHand();
					}
					break;
				}
			}
		}
	}
	
	private void PrintDeck()
	{
		GD.Print($"Current Deck: [{string.Join(", ", _deck.Select(card => card.Name))}]");
	}

	private void PrintHand()
	{
		GD.Print($"Current Hand: [{string.Join(", ", _hand.Select(card => card.Name))}]");
	}

    public override void _Process(double delta)
    {
    }
}
