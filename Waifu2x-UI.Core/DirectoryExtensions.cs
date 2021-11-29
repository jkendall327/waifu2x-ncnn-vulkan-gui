namespace Waifu2x_UI.Core;

public static class DirectoryExtensions
{
    public static DirectoryInfo GetOutputDirectory()
    {
        var fullPath = Path.GetFullPath(Directory.GetCurrentDirectory());
        var separator = Path.DirectorySeparatorChar;

        var path = $"{fullPath}{separator}output";
        Directory.CreateDirectory(path);

        return new(path);
    }
}