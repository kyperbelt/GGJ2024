using Godot;

namespace ComedyDeckBuilder
{
    [Tool]
    public partial class Card : Control
    {
        [Export]
        public CardData Data { get; set; }

        [Export]
        public MoveWithMouse MouseMover { get; set; }

        [Export]
        public AnimationPlayer CardAnimator { get; set; }

        [Export]
        public Sprite2D CardGraphic { get; set; }

        // Called when the node enters the scene tree for the first time.
        public override void _Ready()
        {
        }

        // Called every frame. 'delta' is the elapsed time since the previous frame.
        public override void _Process(double delta)
        {
        }

        public void OnMouseHover()
        {
            CardAnimator.Play("card_hover");
        }
        public void OnMouseLeave()
        {
            CardAnimator.Play("RESET");
        }

        void OnMouseInputEvent(InputEvent ev)
        {

            if (ev is InputEventMouseButton butt)
            {
                if (butt.Pressed)
                {
                    GD.Print("Mouse Clicked");
                    MouseMover.IsFollowingMouse = true;
                }
                else
                {
                    GD.Print("Mouse Released");
                    MouseMover.IsFollowingMouse = false;
                }
            }
        }
    }
}

