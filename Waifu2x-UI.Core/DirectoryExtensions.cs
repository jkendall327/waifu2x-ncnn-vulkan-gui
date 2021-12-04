using System.Reflection;

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

    public static DirectoryInfo GetWaifuDirectory()
    {
        var current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ??
                      throw new DirectoryNotFoundException();
        
        var parent = Directory.GetParent(current) ?? 
                     throw new DirectoryNotFoundException();

        return new(parent.FullName + Path.DirectorySeparatorChar + "waifu2x");
    }
}