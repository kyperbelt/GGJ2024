using Godot;
using System;

public partial class HumourIcon : Sprite2D
{
    [Export]
    public Texture2D _bloodTexture;
    
    [Export]
    public Texture2D _phlegmTexture;
    
    [Export]
    public Texture2D _blackBileTexture;
    
    [Export]
    public Texture2D _yellowBileTexture;
    
    public void SetHumour(CardHumor humour)
    {
        Texture = humour switch
        {
            CardHumor.Blood => _bloodTexture,
            CardHumor.Phlegm => _phlegmTexture,
            CardHumor.Black_Bile => _blackBileTexture,
            CardHumor.Yellow_Bile => _yellowBileTexture,
        };
    }
}
