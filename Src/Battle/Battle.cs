using System;
using Godot;

public partial class Battle : Node2D
{
    // private MadLibafier _madLibafier;
    [Export]
    private SpeechBubble _playerSpeechBubble;
    
    [Export]
    private SpeechBubble _hecklerSpeechBubble;

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
    }
    
	
	public override void _UnhandledInput(InputEvent @event)
	{
		if (@event is InputEventKey eventKey)
		{
			if (eventKey.Pressed && eventKey.Keycode == Key.Space)
			{
				var targetBubble = _turnType switch
				{
					TurnType.Player => _playerSpeechBubble,
					TurnType.Heckler => _hecklerSpeechBubble,
					_ => throw new InvalidOperationException($"Unknown turn type: {_turnType}"),
				};
				targetBubble.ShowDialogueLine("The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].");
				_turnType = _turnType == TurnType.Player ? TurnType.Heckler : TurnType.Player;
			}
		}
	}

    public override void _Process(double delta)
    {
    }
}
