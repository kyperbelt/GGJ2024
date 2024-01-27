using Godot;
using System;

public partial class Sprite2DCustom : Godot.Sprite2D
{
    [Export]
    private int _speed = 400;
    
    [Export]
    private float _angularSpeed = Mathf.Pi;

    [Export]
    private string _name2;
    
    public Sprite2DCustom()
    {
    }

    public override void _Ready()
    {
        GD.Print($"Hello World! I'm {_name2}.");
    }

    public override void _Process(double delta)
    {
        Rotation += _angularSpeed * (float)delta;
        
        var velocity = Vector2.Up.Rotated(Rotation) * _speed;

        Position += velocity * (float)delta;
    }
}
