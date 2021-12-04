using System.IO.Abstractions;

namespace Waifu2x_UI.Core;

public class DirectoryService : IDirectoryService
{
    private readonly IDirectory _directory;
    private readonly IDirectoryInfoFactory _directoryInfoFactory;

    public DirectoryService(IDirectory directory, IDirectoryInfoFactory directoryInfoFactory)
    {
        _directory = directory;
        _directoryInfoFactory = directoryInfoFactory;
    }

    public IDirectoryInfo GetOutputDirectory() => _directoryInfoFactory.FromDirectoryName(_directory.GetCurrentDirectory());
    public IDirectoryInfo GetWaifuDirectory()
    {
        var path = _directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "waifu2x";
        return _directoryInfoFactory.FromDirectoryName(path);
    }

    public IDirectoryInfo FromName(string name)
    {
        return _directoryInfoFactory.FromDirectoryName(name);
    }

    public bool Exists(IDirectoryInfo? directory)
    {
        return directory is not null && _directory.Exists(directory.FullName);
    }
}