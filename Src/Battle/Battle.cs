using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2024.Util;
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
    private List<CardData> _discard = new();

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
		if (@event is not InputEventKey eventKey) return;
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
				DrawCard();
				break;
			}
			case Key.P:
			{
				PlayCard();
				break;
			}
			case Key.X:
			{
				EndTurn();
				break;
			}
		}
	}

	private void DrawCard()
	{
		if (_deck.Count == 0)
		{
			// Try to reshuffle discard pile
			_discard.Shuffle();
			_deck.AddRange(_discard);
			_discard.Clear();
			GD.Print("Try to reshuffle discard pile.");
			PrintDiscard();
			PrintDeck();
		}
		
		if (_deck.Count > 0)
		{
			var nextCard = _deck[0];
			_deck.RemoveAt(0);
			GD.Print($"\ud83d\ude80 Draw Card: {nextCard}");
			_hand.Add(nextCard);
			PrintDeck();
			PrintHand();
		}
		else
		{
			GD.Print("Deck and Discard are Empty!");
		}
	}

	private void PlayCard()
	{
		if (_hand.Count == 0)
		{
			GD.PushError("Hand is Empty!");
		}
		else
		{
			var card = _hand[0];
			_hand.RemoveAt(0);
			GD.Print($"\ud83d\ude80 Play Card: {card}");
			_discard.Add(card);
			PrintHand();
			PrintDiscard();
		}
	}

	private void EndTurn()
	{
		// End turn, discard any remaining cards from hand
		GD.Print($"\ud83d\ude80 End Turn");
		foreach (var card in _hand)
		{
			_discard.Add(card);
			GD.Print($"\ud83d\ude80 Discard Card: {card}");
		}
		_hand.Clear();
		PrintHand();
		PrintDiscard();
	}

	private void PrintDeck()
	{
		GD.Print($"Current Deck: [{string.Join(", ", _deck.Select(card => card.Name))}]");
	}

	private void PrintHand()
	{
		GD.Print($"Current Hand: [{string.Join(", ", _hand.Select(card => card.Name))}]");
	}

	private void PrintDiscard()
	{
		GD.Print($"Current Discard: [{string.Join(", ", _discard.Select(card => card.Name))}]");
	}

    public override void _Process(double delta)
    {
    }
}
