using Godot;

namespace ComedyDeckBuilder
{
    public partial class Card : Node2D
    {
        [Export]
        public CardData Data { get; set; }

        [Export]
        public CollisionObject2D Collision { get; set; }

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
    }
}

