using Godot;

public partial class CardData : Resource
{
    [Export]
    public string Name { get; set; }

    [Export]
    public string Description { get; set; }

    [Export]
    public int Cost { get; set; } // TODO - Is there a cost to account for?

    [Export]
    public int Hilarity { get; set; } // The 'funniness' value

    [Export]
    public Texture2D CardGraphic { get; set; }

    [Export]
    public CardNatureEnum CardNature { get; set; }
}
