using Godot;

[GlobalClass]
public partial class GameScreen : Node
{
	[Signal]
	public delegate void ScreenPausedEventHandler();

	[Signal]
	public delegate void ScreenResumedEventHandler();

	[Export]
	private bool _pausePreviousScreen = false;

	[ExportGroup("Config")]
	[Export(PropertyHint.Dir)]
	private string _screenDirectory = "res://Src/Screen";

	private ScreenManager _screenManager;

	public override void _Ready()
	{
	}

	public void SetScreen(string screenName)
	{
		var screen = GetGameScreenFromName(screenName);
		if (screen != null)
		{
			_screenManager.SetScreen(screen);
		}
	}

	public void PushScreen(string screenName)
	{
		var screen = GetGameScreenFromName(screenName);
		if (screen != null)
		{

			_screenManager.PushScreen(screen);
		}
	}

	public void PopScreen()
	{
		_screenManager.PopScreen();
	}

	public void SetScreenPaused(bool paused)
	{
		if (paused)
		{
			EmitSignal(SignalName.ScreenPaused);
		}
		else
		{
			EmitSignal(SignalName.ScreenResumed);
		}
	}

	public bool IsPausePreviousScreen()
	{
		return _pausePreviousScreen;
	}

	public void SetScreenManager(ScreenManager manager)
	{
		_screenManager = manager;
	}

	private GameScreen GetGameScreenFromName(string name)
	{
		var scene = GD.Load<PackedScene>($"{_screenDirectory}/{name}.tscn");
		GD.Print($"Loading screen from {_screenDirectory}/{name}.tscn");
		if (scene == null)
		{
			GD.Print($"Loading screen from {_screenDirectory}/{name}.scn");
			scene = GD.Load<PackedScene>($"{_screenDirectory}/{name}.scn");
		}

		if (scene == null)
		{
			GD.PushError($"GameScreen[{name}] not found at in Dir[{_screenDirectory}].");
			return null;
		}

		var instance = scene.Instantiate();
		if (instance is GameScreen screen)
		{
			return screen;
		}

		GD.PushError($"screen[{name}] was not a valid GameScreen name. tip: Do not include the extension (tscn).");
		return null;
	}
}
