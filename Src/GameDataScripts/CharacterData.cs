using Godot;

[Tool]
[GlobalClass]
public partial class CharacterData: Resource
{
    [Signal]
    public delegate void CurrentConfidenceChangedEventHandler(int oldConfidence, int currentConfidence);

    [Export]
    public string CharacterName{ get; set; }

    [Export(PropertyHint.File, "*.txt")] 
    public string ChracterMadLibsFile { get; set; }

    [Export(PropertyHint.MultilineText)]
    public string Description { get; set; }

    /// <summary>
    /// The image that is displayed when selecting this character.
    [Export]
    public Texture2D FrontImage { get; set; }

    /// <summary>
    /// The image that is displayed when in battle with this character.
    /// </summary>
    [Export] 
    public Texture2D BackImage { get; set; }

    [ExportCategory("Stats")]
    [Export]
    public int MaxConfidence{ get; set; } = 30;

    private int _currentConfidence;
    [Export]
    public int CurrentConfidence
    {
        get => _currentConfidence;
        set
        {
            if (value > MaxConfidence)
            {
                _currentConfidence = MaxConfidence;
                GD.Print("Overconfidence!");
            }
            else if (value < 0)
            {
                _currentConfidence = 0;
            }
            else
            {
                _currentConfidence = value;
            }
            EmitSignal(nameof(CurrentConfidenceChanged), _currentConfidence);
        }
    }

    public void _SetCurrentConfidence(int value)
    {
        _currentConfidence = value;
        EmitSignal(nameof(CurrentConfidenceChanged), _currentConfidence);
    }

    public int GetCurrentConfidence()
    {
        return _currentConfidence;
    }

}
