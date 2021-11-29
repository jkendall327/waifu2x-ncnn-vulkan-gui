namespace Waifu2x_UI.Core;

public enum OutputFileType
{
    Jpeg,
    Png
}

public static class EnumExtensions
{
    public static string ToExtension(this OutputFileType type)
    {
        return type switch
        {
            OutputFileType.Jpeg => "jpg",
            OutputFileType.Png => "png",
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };
    }
}