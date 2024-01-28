using System.Runtime.CompilerServices;


/// <summary>
/// Named after the medieval 'humors'
/// </summary>
public enum CardHumorTypeEnum
{
    None = 0, // no value
    Phlegm = 1,
    Blood = 2,
    Yellow_Bile = 3,
    Black_Bile = 4,
    Error = 999,

}

public static class CardHumorTypeExtension
{

    public static string ToLabelString(this CardHumorTypeEnum _humor)
    {
        if (_humor == CardHumorTypeEnum.None)
            return string.Empty;
        return _humor.ToString().Replace('_', ' ');
    }
}