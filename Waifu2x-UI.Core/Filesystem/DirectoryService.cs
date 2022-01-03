using System.IO.Abstractions;
using System.Reflection;
using Microsoft.Extensions.Logging;

namespace Waifu2x_UI.Core.Filesystem;

public class DirectoryService : IDirectoryService
{
    private readonly IDirectory _directory;
    private readonly IDirectoryInfoFactory _directoryInfoFactory;
    private readonly ILogger<DirectoryService> _logger;

    public DirectoryService(IDirectory directory, IDirectoryInfoFactory directoryInfoFactory, ILogger<DirectoryService> logger)
    {
        _directory = directory;
        _directoryInfoFactory = directoryInfoFactory;
        _logger = logger;
    }

    public IDirectoryInfo GetOutputDirectory() => _directoryInfoFactory.FromDirectoryName(_directory.GetCurrentDirectory());
    public IDirectoryInfo GetWaifuDirectory()
    {
        var assemblyPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

        if (string.IsNullOrEmpty(assemblyPath))
        {
            _logger.LogError("An error occured when determining the directory for the application assembly");
            throw new DirectoryNotFoundException();
        }

        var path = Path.Combine(assemblyPath, "waifu2x");
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