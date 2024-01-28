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

    public Vector2 SnapBackPosition;
    public bool IsSnapBackSet { get; set;} = false;

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
    }

    public void SnapBack()
    {
        if (UseSpawnPointForSnapBack)
        {
            GlobalPosition = SnapBackPosition;
        }
        else
        {
            GlobalPosition = new Vector2(0, 0);
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
        // else if (WillSnapBackOnRelease && IsSnapBackSet)
        // {
        //     GlobalPosition = SnapBackPosition;
        // }

    }
}
