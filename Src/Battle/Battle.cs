using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GGJ2024.Util;
using Godot;

public partial class Battle : Node2D
{
	[Signal]
	public delegate void MaterialCostChangedEventHandler(int oldValue, int newValue);

	[Signal]
	public delegate void BattleLostEventHandler();

	[Signal]
	public delegate void BattleWonEventHandler();

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
	private HumourIcon _crowdFavoriteHumourIcon;
	
	[Export]
	private HumourIcon _crowdDislikedHumourIcon;

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
	private int _crowdFavoriteHumourMultiplier = 1;
	private bool _playerJustTookDamage = false;
	
	// Crowd stats
	private CardHumor _crowdFavoriteHumour;
	private CardHumor _crowdDislikedHumour;
	
	// Sound players
	[Export]
	private RandomAudioPlayer _crowdLaughPlayer;
	
	[Export]
	private RandomAudioPlayer _crowdBooPlayer;
	
	[Export]
	private RandomAudioPlayer _crowdHecklerReactionPlayer;
	
	[Export]
	private AudioStreamPlayer _cardFlipPlayer;
	
	[Export]
	private AudioStreamPlayer _cardSlapPlayer;
	
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
	private const int CrowdFavoriteHumourMultiplier = 2;

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
			var randomCardProto = _cardPrototypes.RandomElement();
			_deck.Add(randomCardProto);
		}
		
		// Randomize crowd's favorite/disliked humours
		_crowdFavoriteHumour = CardEnumExtension.RandomCardHumour();
		// Ensure disliked is unique from favorite
		do
		{
			_crowdDislikedHumour = CardEnumExtension.RandomCardHumour();
		} while (_crowdDislikedHumour == _crowdFavoriteHumour);
		_crowdFavoriteHumourIcon.SetHumour(_crowdFavoriteHumour);
		_crowdDislikedHumourIcon.SetHumour(_crowdDislikedHumour);
		GD.Print($"Picked Crowd Favorite Humour: {_crowdFavoriteHumour}, Crowd Disliked Humour: {_crowdDislikedHumour}");

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
				async (i, pos) =>
				{
					if (!_hecklerArea.GetGlobalRect().HasPoint(pos) || !await PlayCard(i))
					{
					   _handArea.GetChild<Card>(i).CardVisual.MouseMover.SnapBack(); 
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
				// Debug button
				ShowMadLibsSpeechBubble("[noun] stop.");
				break;
			case Key.D:
				DrawCard();
				break;
			case Key.P:
				PlayCard(0);
				break;
			case Key.X:
				EndPlayerTurn();
				break;
			case Key.W:
				EmitSignal(SignalName.BattleWon);
				break;
			case Key.L:
				EmitSignal(SignalName.BattleLost);
				break;
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
		
		// Play a random card from the list of all card types with simplified play logic
		var randomCardProto = _cardPrototypes.RandomElement();
		GD.Print($"Heckler does {randomCardProto.Hilarity} damage to player.");
		if (randomCardProto.Hilarity > 0)
		{
			_playerCharacter.CurrentConfidence -= randomCardProto.Hilarity;
			_playerJustTookDamage = true;
			_crowdHecklerReactionPlayer.PlayRandom();
		}
		if (_playerCharacter.CurrentConfidence <= 0)
		{
			EmitSignal(SignalName.BattleLost);
			GD.Print($"\ud83d\udca5\ud83d\udca5\ud83d\udca5 You lose!");
			return;
		}
		
		ShowMadLibsSpeechBubble(randomCardProto.MadlibSentence);
		
		await ToSignal(GetTree().CreateTimer(1), "timeout");
		GD.Print($"\ud83d\ude80 End Heckler Turn");
		StartPlayerTurn();
	}

	private async void DrawHand()
	{
		for (int i = 0; i < DrawAmount; ++i)
		{
			await DrawCard();
			GD.Print($"Draw Card {i}");

		}
		PrintDeck();
		PrintHand();
	}

	private async Task<int> DrawCard()
	{
		if (Engine.IsEditorHint()) return 0;
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
			_cardFlipPlayer.Play();
			await ToSignal(_handArea, HandArea.SignalName.CardDrawnAnimationFinished);
		}
		else
		{
			GD.Print("Tried to draw a card but deck and discard are empty!");
		}
		return 0;
	}

	private async Task<bool> PlayCard(int cardToPlayInHandIndex)
	{
		if (_hand.Count == 0)
		{
			GD.PushError("Play Card: Hand is Empty!");
			return false;
		}

		var card = _hand[cardToPlayInHandIndex];

		// Check if we can afford this card
		if (card.Cost > MaterialAmount)
		{
			GD.Print($"Play Card: Can't afford card. Cost: {card.Cost} Material:{MaterialAmount}");
			ShowCantAffordSpeechBubble();
			return false;
		}

		// Play card.
		MaterialAmount -= card.Cost;

		GD.Print($"\ud83d\ude80 Play Card Succeeded: {card}");
		ShowMadLibsSpeechBubble(card.MadlibSentence);

		await DiscardCard(cardToPlayInHandIndex, card);
		
		PrintPlayerStats();
		
		var damageMultiplier = 1;
		
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
				damageMultiplier += _setupMultiplier;
				_setupMultiplier = 1;
				break;
			}
			case CardType.Comeback:
			{
				if (_playerJustTookDamage)
				{
					GD.Print("Player gets comeback bonus!");
					damageMultiplier += ComebackDamageMultiplier;
				}
				break;
			}
		}
		
		_playerJustTookDamage = false;
		
		// Check if crowd dislikes humour type. If so, heal heckler instead of damaging.
		if (_crowdDislikedHumour == card.CardNature)
		{
			GD.Print($"Crowd dislikes humour type:{card.CardNature}. Healing Heckler instead of damaging.");
			damageMultiplier *= -1;
		}
		
		// Apply crowd favorite humour multiplier from last turn if applicable.
		if (_crowdFavoriteHumourMultiplier != 1)
		{
			GD.Print($"Apply crowd favorite humour multiplier from last turn = {_crowdFavoriteHumourMultiplier}");
			damageMultiplier += _crowdFavoriteHumourMultiplier;
			_crowdFavoriteHumourMultiplier = 1;
		}
		
		// Do damage
		var cardDamage = card.Hilarity * damageMultiplier;
		GD.Print($"Player did {cardDamage} damage to heckler (base damage = {card.Hilarity}, multiplier = {damageMultiplier}).");
		_hecklerCharacter.CurrentConfidence -= cardDamage;
		if (_hecklerCharacter.CurrentConfidence <= 0)
		{
			EmitSignal(SignalName.BattleWon);
			GD.Print($"\ud83c\udf89\ud83c\udf89\ud83c\udf89 You win!");
			return true;
		}

		// Check if crowd likes humour type. If so, add multiplier on next turn.
		if (_crowdFavoriteHumour == card.CardNature)
		{
			GD.Print($"Played crowd favorite humour: {_crowdFavoriteHumour}. Multiplier of {CrowdFavoriteHumourMultiplier} will apply on next played card.");
			_crowdFavoriteHumourMultiplier = CrowdFavoriteHumourMultiplier;
		}
		
		// Play crowd reaction sound effect
		if (_crowdDislikedHumour == card.CardNature)
		{
			_crowdBooPlayer.PlayRandom();
		}
		else
		{
			_crowdLaughPlayer.PlayRandom();
		}
		_cardSlapPlayer.Play();

		PrintHand();
		PrintDiscard();
		PrintPlayerStats();
		return true;
	}

	private async Task<int> DiscardCard(int index, CardData data)
	{
		_hand.RemoveAt(index);
		_handArea.DiscardCard(index);
		await ToSignal(_handArea, HandArea.SignalName.CardDiscardAnimationFinished);
		_discard.Add(data);
		_discardPileUx.DiscardPileNumber = _discard.Count;
		return 0;
	}

	private async void EndPlayerTurn()
	{
		if (_turnType != TurnType.Player) return;
		// End turn, discard any remaining cards from hand
		GD.Print($"\ud83d\ude80 End Player Turn");
		for(int i = _hand.Count-1; i >= 0; i--)
		{
			var card = _hand[i];
			GD.Print($"\ud83d\ude80 Discard Card: {card}");
			_handArea.DiscardCard(i);
			await ToSignal(_handArea, HandArea.SignalName.CardDiscardAnimationFinished);
			_discard.Add(card);
			_discardPileUx.DiscardPileNumber = _discard.Count;
		}
		_hand.Clear();
		PrintHand();
		PrintDiscard();

		StartHecklerTurn();
	}

	private void PrintPlayerStats()
	{
		GD.Print($"Player Stats: Material: {_material} SetupMultiplier: {_setupMultiplier} CrowdFavoriteHumourMultiplier: {_crowdFavoriteHumourMultiplier}");
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
