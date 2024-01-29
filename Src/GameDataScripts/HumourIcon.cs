using Godot;
using System;
using GGJ2024.GameDataScripts;

public partial class HumourIcon : Sprite2D
{
    [Export]
    private HumourIconTexture _humourIconTexture;

    public void SetHumour(CardHumor humour)
    {
        Texture = _humourIconTexture.GetTextureForHumour(humour);
    }
}
