using System;
using System.Collections.Generic;
using System.Linq;
using GGJ2024.Util;
using Godot;

public partial class Battle : Node2D
{
    [Signal]
    public delegate void MaterialCostChangedEventHandler(int oldValue, int newValue);

    // private MadLibafier _madLibafier;
    [Export]
    private CharacterComic _playerCharacter;
    
    [Export]
    private CharacterComic _hecklerCharacter;
    
    [Export]
    private SpeechBubble _playerSpeechBubble;

    [Export]
    private SpeechBubble _hecklerSpeechBubble;

    [Export]
    private CardData[] _cardPrototypes;

    [Export]
    private HandArea _handArea;

    private ReferenceRect _hecklerArea;

    private List<CardData> _deck = new();
    private List<CardData> _hand = new();
    private List<CardData> _discard = new();

    // Additional player stats
    private int _material = MaterialPerTurn;
    private int _setupMultiplier = 1;
    private bool _playerJustTookDamage = false;
    
    public int MaterialAmount
    {
        get => _material;
        set
        {
            EmitSignal(SignalName.MaterialCostChanged, _material, value);
            _material = value;
        }
    }

    private Material _materialObjectUx;
    private DrawButton _drawDeckPileUx;
    private DiscardButton _discardPileUx;

    [Export]
    private int _deckSize = 64;

    // Game rule constants
    private const int DrawAmount = 5;
    private const int MaterialPerTurn = 5;
    private const int ComebackDamageMultiplier = 2;

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

        _hecklerArea = GetNode<ReferenceRect>("HecklerArea");

        // Create randomized deck
        for (int i = 0; i < _deckSize; ++i)
        {
            var randomCardProtoIndex = Random.Shared.Next(0, _cardPrototypes.Length);
            var randomCardProto = _cardPrototypes[randomCardProtoIndex];
            _deck.Add(randomCardProto);
        }

        _materialObjectUx = GetNode<Material>("Material");
        _materialObjectUx.MaxMaterialValue = MaterialPerTurn;
        _materialObjectUx.MaterialValue = MaterialAmount;
        Connect(SignalName.MaterialCostChanged,
             Callable.From<int, int>((x, y) =>
             {
                 _materialObjectUx.MaterialValue = y;
             }));

        _discardPileUx = GetNode<DiscardButton>("DiscardButton");
        _discardPileUx.DiscardPileNumber = 0;

        _drawDeckPileUx = GetNode<DrawButton>("DrawButton");
        _drawDeckPileUx.DrawPileNumber = _deck.Count;

        if (_handArea != null)
        {
            _handArea.Connect(HandArea.SignalName.CardPlayed, Callable.From<int, Vector2>(
                (i, pos) =>
                {
		    GD.Print("Heckler Area Clicked" + pos);
		    GD.Print("Heckler Area Bounds" + _hecklerArea.GetGlobalRect());
		    GD.Print("Heckler Area Clicked" + _hecklerArea.GetGlobalRect().HasPoint(pos));
                    if (_hecklerArea.GetGlobalRect().HasPoint(pos))
                    {
                        PlayCard(i);

                    }
                }));
        }

        StartPlayerTurn();
    }


    public override void _UnhandledInput(InputEvent @event)
    {
        if (Engine.IsEditorHint()) return;
        if (@event is not InputEventKey eventKey) return;
        if (!eventKey.IsReleased()) return;
        switch (eventKey.Keycode)
        {
            case Key.Space:
                {
                    // Debug button
                    ShowMadLibsSpeechBubble("[noun] stop.");
                    break;
                }
            case Key.D:
                {
                    DrawCard();
                    break;
                }
            case Key.P:
                {
                    PlayCard(0);
                    break;
                }
            case Key.X:
                {
                    EndPlayerTurn();
                    break;
                }
        }
    }

    private void ShowMadLibsSpeechBubble(string madLibString)
    {
        var targetBubble = _turnType switch
        {
            TurnType.Player => _playerSpeechBubble,
            TurnType.Heckler => _hecklerSpeechBubble,
            _ => throw new InvalidOperationException($"Unknown turn type: {_turnType}"),
        };
        // "The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun]."
        targetBubble.ShowDialogueLine(madLibString);
    }

    private void ShowCantAffordSpeechBubble()
    {
        _playerSpeechBubble.ShowDialogueLine("I can't afford that card.");
    }

    private void StartPlayerTurn()
    {
        GD.Print("\ud83d\ude80 Start Player Turn");
        _turnType = TurnType.Player;
        MaterialAmount = MaterialPerTurn;
        DrawHand();
        PrintPlayerStats();
    }

    private async void StartHecklerTurn()
    {
        // TODO implement real version
        GD.Print($"\ud83d\ude80 Start Heckler Turn");
        _turnType = TurnType.Heckler;
        await ToSignal(GetTree().CreateTimer(1), "timeout");
        ShowMadLibsSpeechBubble("The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].");
        
        // Temp logic for doing damage on heckler turn
        var randomDamage = Random.Shared.Next(0, 6);
        GD.Print($"Heckler does {randomDamage} damage to player.");
        if (randomDamage > 0)
        {
            _playerCharacter.CurrentConfidence -= randomDamage;
            _playerJustTookDamage = true;
        }
        if (_playerCharacter.CurrentConfidence <= 0)
        {
            GD.Print($"\ud83d\udca5\ud83d\udca5\ud83d\udca5 You lose!");
            return;
        }
        
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

            _drawDeckPileUx.DrawPileNumber = _deck.Count;
            _discardPileUx.DiscardPileNumber = _discard.Count;

            _discard.Clear();
            GD.Print("Try to reshuffle discard pile.");
            PrintDiscard();
            PrintDeck();
        }

        if (_deck.Count > 0)
        {
            var nextCard = _deck[0];
            _deck.RemoveAt(0);
            _drawDeckPileUx.DrawPileNumber = _deck.Count;
            GD.Print($"\ud83d\ude80 Draw Card: {nextCard}");
            _hand.Add(nextCard);
            _handArea.DrawCard(nextCard);
        }
        else
        {
            GD.Print("Tried to draw a card but deck and discard are empty!");
        }
    }

    private void PlayCard(int cardToPlayInHandIndex)
    {
        if (_hand.Count == 0)
        {
            GD.PushError("Play Card: Hand is Empty!");
            return;
        }

        var card = _hand[cardToPlayInHandIndex];

        // Check if we can afford this card
        if (card.Cost > MaterialAmount)
        {
            GD.Print($"Play Card: Can't afford card. Cost: {card.Cost} Material:{MaterialAmount}");
            ShowCantAffordSpeechBubble();
            return;
        }

        // Play card.
        MaterialAmount -= card.Cost;

        GD.Print($"\ud83d\ude80 Play Card Succeeded: {card}");
        ShowMadLibsSpeechBubble(card.MadlibSentence);
        
        DiscardCard(cardToPlayInHandIndex, card);

        PrintPlayerStats();
        
        var cardDamage = card.Hilarity;
        
        // Check for special card type effects
        switch (card.CardType)
        {
            case CardType.Set_Up:
            {
                _setupMultiplier += 1;
                break;
            }
            case CardType.Punchline:
            {
                cardDamage *= _setupMultiplier;
                _setupMultiplier = 1;
                break;
            }
            case CardType.Comeback:
            {
                if (_playerJustTookDamage)
                {
                    GD.Print("Player gets comeback bonus!");
                    cardDamage *= ComebackDamageMultiplier;
                }
                break;
            }
        }
        
        _playerJustTookDamage = false;
        
        // Do damage
        GD.Print($"Player did {cardDamage} damage to heckler (base damage = {card.Hilarity}).");
        _hecklerCharacter.CurrentConfidence -= cardDamage;
        if (_hecklerCharacter.CurrentConfidence <= 0)
        {
            GD.Print($"\ud83c\udf89\ud83c\udf89\ud83c\udf89 You win!");
            return;
        }
        // TODO: process card's effects on audience

        PrintHand();
        PrintDiscard();
        PrintPlayerStats();
    }

    private void DiscardCard(int index, CardData data)
    {
        _hand.RemoveAt(index);
        _handArea.DiscardCard(index);
        _discard.Add(data);
        _discardPileUx.DiscardPileNumber = _discard.Count;
    }

    private void EndPlayerTurn()
    {
        // End turn, discard any remaining cards from hand
        GD.Print($"\ud83d\ude80 End Player Turn");
        int i = 0;
        foreach (var card in _hand)
        {
            GD.Print($"\ud83d\ude80 Discard Card: {card}");
            _handArea.DiscardCard(i);
            _discard.Add(card);
            _discardPileUx.DiscardPileNumber = _discard.Count;
            i++;
        }
        _hand.Clear();
        PrintHand();
        PrintDiscard();

        StartHecklerTurn();
    }

    private void PrintPlayerStats()
    {
        GD.Print($"Player Stats: Material: {_material} SetupMultiplier: {_setupMultiplier}");
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
