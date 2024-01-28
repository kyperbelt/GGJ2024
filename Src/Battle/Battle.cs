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
    private int _material = 0;

    [Export]
    private int _deckSize = 64;

    private const int DrawAmount = 5;
    private const int MaterialPerTurn = 5;

    private enum TurnType
    {
        Player,
        Heckler
    }
    private TurnType _turnType = TurnType.Player;

    public override void _Ready()
    {
        if (Engine.IsEditorHint()) return;
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
		
        StartPlayerTurn();
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        if (@event is not InputEventKey eventKey) return;
        if (!eventKey.Pressed) return;
        switch (eventKey.Keycode)
        {
            case Key.Space:
                {
                    // Debug button
				ShowMadLibsSpeechBubble();
                    break;
                }
            case Key.D:
                {
                    DrawHand();
                    break;
                }
            case Key.P:
                {
                    PlayCard();
                    break;
                }
            case Key.X:
                {
                    EndPlayerTurn();
                    break;
                }
        }
    }

    private void ShowMadLibsSpeechBubble()
	{
		var targetBubble = _turnType switch
		{
			TurnType.Player => _playerSpeechBubble,
			TurnType.Heckler => _hecklerSpeechBubble,
			_ => throw new InvalidOperationException($"Unknown turn type: {_turnType}"),
		};
		targetBubble.ShowDialogueLine("The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].");
	}
	
	private void ShowCantAffordSpeechBubble()
	{
		_playerSpeechBubble.ShowDialogueLine("I can't afford that card.");
	}

	private void StartPlayerTurn()
	{
		GD.Print("\ud83d\ude80 Start Player Turn");
		_turnType = TurnType.Player;
		_material = MaterialPerTurn;
		DrawHand();
		PrintPlayerStats();
	}
	
	private async void StartHecklerTurn()
	{
		// TODO implement real version
		GD.Print($"\ud83d\ude80 Start Heckler Turn");
		_turnType = TurnType.Heckler;
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		ShowMadLibsSpeechBubble();
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GD.Print($"\ud83d\ude80 End Heckler Turn");
		StartPlayerTurn();
	}

	private void DrawHand()
	{
		GD.Print("Draw new hand.");
		for (int i = 0; i < DrawAmount; ++i)
		{
			DrawCard();
		}
		PrintDeck();
		PrintHand();
	}
	
	private void DrawCard()
	{
		if (Engine.IsEditorHint()) return;
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
		}
		else
		{
			GD.Print("Tried to draw a card but deck and discard are empty!");
        }
    }

    private void PlayCard()
    {
        if (_hand.Count == 0)
        {
            GD.PushError("Play Card: Hand is Empty!");
			return;
        }

		// TODO: let player choose a card. Use first card in hand for now.
        const int cardToPlayInHandIndex = 0;
		var card = _hand[cardToPlayInHandIndex];
			
		// Check if we can afford this card
		if (card.Cost > _material)
		{
			GD.Print($"Play Card: Can't afford card. Cost: {card.Cost} Material:{_material}");
			ShowCantAffordSpeechBubble();
			return;
		}

		// Play card.
		_material -= card.Cost;
		_hand.RemoveAt(cardToPlayInHandIndex);
		
		// TODO: process card's effects
			
		GD.Print($"\ud83d\ude80 Play Card Succeeded: {card}");
		_discard.Add(card);
		ShowMadLibsSpeechBubble();
		
		PrintHand();
		PrintDiscard();
		PrintPlayerStats();
    }

    private void EndPlayerTurn()
    {
        // End turn, discard any remaining cards from hand
        GD.Print($"\ud83d\ude80 End Player Turn");
		foreach (var card in _hand)
		{
			_discard.Add(card);
			GD.Print($"\ud83d\ude80 Discard Card: {card}");
		}
		_hand.Clear();
		PrintHand();
		PrintDiscard();
		
		StartHecklerTurn();
	}

	private void PrintPlayerStats()
	{
		GD.Print($"Player Stats: Material: {_material}");
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
