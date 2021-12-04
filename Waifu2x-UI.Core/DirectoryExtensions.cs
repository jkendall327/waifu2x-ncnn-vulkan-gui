using System.IO.Abstractions;

namespace Waifu2x_UI.Core;

public static class DirectoryExtensions
{
    public static DirectoryInfo GetOutputDirectory(this IDirectory d) => new(d.GetCurrentDirectory());
    public static DirectoryInfo GetWaifuDirectory(this IDirectory d) => new(d.GetCurrentDirectory() + Path.DirectorySeparatorChar + "waifu2x");
}