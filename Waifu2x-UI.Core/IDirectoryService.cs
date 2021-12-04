using System.IO.Abstractions;

namespace Waifu2x_UI.Core;

/// <summary>
/// Exposes convenience methods for getting app-relevant directories.
/// </summary>
public interface IDirectoryService
{
    /// <summary>
    /// Get the default output directory for processed images.
    /// </summary>
    public IDirectoryInfo GetOutputDirectory();
    
    /// <summary>
    /// Gets the directory where the Waifu2x executable should be found.
    /// </summary>
    public IDirectoryInfo GetWaifuDirectory();
    
    /// <summary>
    /// Converts a <see cref="string"/> into a <see cref="IDirectoryInfo"/>.
    /// </summary>
    IDirectoryInfo FromName(string name);

    /// <summary>
    /// Determines whether or not a given <see cref="IDirectoryInfo"/> exists.
    /// </summary>
    /// <returns>true, if the directory exists; otherwise, false.</returns>
    bool Exists(IDirectoryInfo? directory);
}