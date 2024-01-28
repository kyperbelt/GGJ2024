using Godot;

[GlobalClass]
public partial class CardData : Resource
{
    [Export]
    public string Name { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    [Export]
    public int Cost { get; set; } // TODO - Is there a cost to account for?

    [Export]
    public int Hilarity { get; set; } // The 'funniness' value

    [Export]
    public Texture2D CardIcon { get; set; }

    [Export]
    public CardHumorTypeEnum CardNature { get; set; }

    public override string ToString()
    {
        return $"{base.ToString()}, {nameof(Name)}: {Name}, {nameof(Cost)}: {Cost}, {nameof(Hilarity)}: {Hilarity}, {nameof(CardNature)}: {CardNature}";
    }
}
