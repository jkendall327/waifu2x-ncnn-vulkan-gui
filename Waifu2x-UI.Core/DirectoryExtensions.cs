using System.IO.Abstractions;

namespace Waifu2x_UI.Core;

public static class DirectoryExtensions
{
    public static string GetOutputDirectory(this IDirectory d) => d.GetCurrentDirectory();
    public static string GetWaifuDirectory(this IDirectory d) => d.GetCurrentDirectory() + Path.DirectorySeparatorChar + "waifu2x";
}