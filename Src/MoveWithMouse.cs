using Godot;

[GlobalClass]
public partial class MoveWithMouse : Control
{

    [Export]
    public bool IsFollowingMouse { get; set; }

    [Export]
	public Vector2 MousePosition { get; private set; }

    [Export]
    public bool WillSnapBackOnRelease = true;

    [Export]
    public bool UseSpawnPointForSnapBack = true;

    private Vector2 SnapBackPosition;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
	{
        if (UseSpawnPointForSnapBack)
        {
            SnapBackPosition = GlobalPosition;
        }
    }

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
        MousePosition = GetGlobalMousePosition();
        if (IsFollowingMouse)
        {
            GlobalPosition = MousePosition;
        }
        else if (WillSnapBackOnRelease)
        {
            GlobalPosition = SnapBackPosition;
        }

	}
}
