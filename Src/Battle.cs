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

        string testString = "The [adjective] [noun] [verb.ing] [preposition] the [adjective] [noun].";
        // test madlibifier 
        GD.Print(_madLibafier.GetMadLib(testString));
        for(int i = 0; i < 100; i++)
        {
            GD.Print(_madLibafier.GetMadLib(testString));
        } 
    }

    public override void _Process(double delta)
    {
    }
}
