using Godot;

namespace GGJ2024.GameDataScripts;

[Tool]
[GlobalClass]
public partial class HumourIconTexture: Resource
{
    [Export]
    public Texture2D _bloodTexture;
    
    [Export]
    public Texture2D _phlegmTexture;
    
    [Export]
    public Texture2D _blackBileTexture;
    
    [Export]
    public Texture2D _yellowBileTexture;
    
    public Texture2D GetTextureForHumour(CardHumor humour)
    {
        return humour switch
        {
            CardHumor.Blood => _bloodTexture,
            CardHumor.Phlegm => _phlegmTexture,
            CardHumor.Black_Bile => _blackBileTexture,
            CardHumor.Yellow_Bile => _yellowBileTexture,
        };
    }
}