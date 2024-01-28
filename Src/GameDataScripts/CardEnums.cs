 using System;
 using Godot;
using System.Runtime.CompilerServices;


/// <summary>
/// Named after the medieval 'humors'
/// </summary>
public enum CardHumor
{
    None = 0, // no value
    Phlegm = 1,
    Blood = 2,
    Yellow_Bile = 3,
    Black_Bile = 4,
    Error = 999,
}

public enum CardType
{
    None = 0, // no value
    Set_Up = 1,
    Comeback = 2,
    Punchline = 3,
    One_Liner = 4,
    Error = 999,

}

public static class CardEnumExtension
{
    public static CardHumor RandomCardHumour()
    {
        var randomIndex = Random.Shared.Next(0, 4);
        return randomIndex switch
        {
            0 => CardHumor.Blood,
            1 => CardHumor.Phlegm,
            2 => CardHumor.Yellow_Bile,
            3 => CardHumor.Black_Bile,
            _ => throw new InvalidOperationException(
                $"Unexpected random number '{randomIndex}' when generating {nameof(RandomCardHumour)}"),
        };
    }

    public static string ToLabelString(this CardType _type)
    {
        switch(_type)
        {
            case CardType.None:
                return string.Empty;
            case CardType.One_Liner:
                return "One-Liner";
            default:
                return _type.ToString().Replace('_', ' ');
        }
    }

    public static string ToLabelString(this CardHumor _humor)
    {
        switch (_humor)
        {
            case CardHumor.None:
                return string.Empty;
            default:
                return _humor.ToString().Replace('_', ' ');
        }
    }

    public static Color ToColor(this CardHumor _humor)
    {
        switch(_humor)
        {
            case CardHumor.Phlegm:
                return Color.Color8(136,156,155,255);
            case CardHumor.Blood:
                return Color.Color8(107, 47, 57, 255);
            case CardHumor.Yellow_Bile:
                return Color.Color8(147, 143, 113, 255);
            case CardHumor.Black_Bile:
                return Color.Color8(42, 42, 43, 255);
            case CardHumor.Error:
                return Colors.Red;
            default:
                return Colors.Transparent;
        }
    }
}