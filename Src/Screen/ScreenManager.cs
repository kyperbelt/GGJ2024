using System.Collections.Generic;
using Godot;

[GlobalClass]
[Tool]
public partial class ScreenManager : Node
{
	private readonly Stack<GameScreen> _screenStack;

	public ScreenManager()
	{
		_screenStack = new Stack<GameScreen>();
	}

	public override void _Ready()
	{
		// GUARD
		if (Engine.IsEditorHint())
		{
			return;
		}

		AddToGroup("ScreenManager");

		// Look for the first GameScreen that is a child of this manager
		// to use as a starting screen.
		var startingScreen = GetStartingScreen();

		if (startingScreen != null)
		{
			PushScreen(startingScreen);
		}
		else
		{
			GD.PushError("No Starting Game Screen Detected");
		}
	}

	private GameScreen GetStartingScreen()
	{
		for (int i = 0; i < GetChildCount(); i++)
		{
			if (GetChild(i) is GameScreen screen)
				return screen;
		}
		return null;
	}

	public override string[] _GetConfigurationWarnings()
	{
		var warnings = new List<string>();

		if (GetStartingScreen() == null)
		{
			warnings.Add("No starting screen found.");
		}

		return warnings.ToArray();
	}

	public void PushScreen(GameScreen screen)
	{
		if (_screenStack.Count > 0 && screen.IsPausePreviousScreen())
		{
			_screenStack.Peek().SetScreenPaused(true);
			_screenStack.Peek().ProcessMode = ProcessModeEnum.Disabled;
			GD.Print($"'{_screenStack.Peek().Name}' has been paused.");
		}

		_screenStack.Push(screen);
		screen.SetScreenManager(this);
		if (screen.GetParent() == null)
		{
			AddChild(screen);
		}

		GD.Print($"'{screen.Name}' pushed to the top of the Screen Stack!");
	}

	public void PopScreen()
	{
		if (_screenStack.Count == 0)
		{
			GD.Print("Unable to pop when there are no screens on the Screen Stack.");
			return;
		}

		var screen = _screenStack.Pop();
		GD.Print($"'{screen.Name}' has been popped from the Screen Stack!");
		if (screen.IsPausePreviousScreen() && _screenStack.Count > 0)
		{
			_screenStack.Peek().SetScreenPaused(false);
			_screenStack.Peek().ProcessMode = ProcessModeEnum.Inherit;
			GD.Print($"'{_screenStack.Peek().Name}' has been unpaused.");
		}

		screen.QueueFree();
	}

	public void SetScreen(GameScreen screen)
	{
		while (_screenStack.Count > 0)
		{
			PopScreen();
		}

		PushScreen(screen);
	}

}
