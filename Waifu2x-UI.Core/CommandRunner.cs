using System.Diagnostics;
using Microsoft.Extensions.Logging;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    private readonly string _waifuPath;

    private readonly IDirectoryService _directoryService;
    private readonly ILogger<CommandRunner> _logger;
    
    public CommandRunner(IDirectoryService directoryService, ILogger<CommandRunner> logger)
    {
        _directoryService = directoryService;
        _logger = logger;

        _waifuPath = GetWaifu();
    }
    
    public async IAsyncEnumerable<(string, double)> Run(Command command)
    {
        var files = command.InputImages;

        using var scope = _logger.BeginScope("Processing {ImageCount} images", files.Count);
        
        var total = files.Count;
        var counter = 0;
        
        var args = command.GenerateArguments();

        foreach (var file in files)
        {
            counter++;
            
            var finalArgs = args.Replace("$IMAGE", file.Name);

            var output = await SpawnProcess(finalArgs);

            var report = $"({counter}/{total}) {output}";

            _logger.LogInformation("Processed {Count}/{Total}", counter, total);
            
            var percentage = CalculatePercentage(counter, total);
            
            yield return (report, percentage);
        }
    }

    private double CalculatePercentage(int counter, int total) => (double)(counter * 100) / total;

    private async Task<string> SpawnProcess(string args)
    {
        using var scope = _logger.BeginScope("Spawning process with args {ProcessArgs}", args);
            
        var processStartInfo = new ProcessStartInfo
        {
            FileName = _waifuPath,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };

        var process = new Process
        {
            StartInfo = processStartInfo
        };
        
        process.Start();

        var id = process.Id;
        
        _logger.LogInformation("Process {ProcessID} starting", id);

        var result = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();
        
        _logger.LogInformation("Process {ProcessID} ended", id);

        return result;
    }

    private string GetWaifu()
    {
        var file = _directoryService
            .GetWaifuDirectory()
            .EnumerateFiles()
            .FirstOrDefault(x => x.Name is "waifu2x-ncnn-vulkan");

        if (file is null) throw new FileNotFoundException("Waifu2x executable not found!");
        
        _logger.LogInformation("Loaded Waifu2x executable from {ExecutablePath}", file.FullName);
        return file.FullName;
    }
}