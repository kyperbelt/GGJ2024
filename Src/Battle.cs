using Godot;

public partial class Battle : Node2D
{
    private MadLibafier _madLibafier;
    public override void _Ready()
    {
        _madLibafier = GetNode<MadLibafier>("MadLibafier");

        if (_madLibafier == null)
        {
            GD.PushError("MadLibafier not found in Battle scene");
        }

        // test madlibifier 
        string testString = "The [adjective] [noun] [verb] [preposition] the [adjective] [noun].";
        GD.Print(_madLibafier.GetMadLib(testString));
    }

    public override void _Process(double delta)
    {
    }
}
