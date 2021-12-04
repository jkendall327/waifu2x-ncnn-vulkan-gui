using System.Diagnostics;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    private readonly string _waifuPath;

    private readonly IDirectoryService _directoryService;
    
    public CommandRunner(IDirectoryService directoryService)
    {
        _directoryService = directoryService;

        _waifuPath = GetWaifu();
    }
    
    public async IAsyncEnumerable<(string, double)> Run(Command command)
    {
        var files = command.InputImages;

        var total = files.Count;
        var counter = 0;
        
        var args = command.GenerateArguments();

        foreach (var file in files)
        {
            counter++;
            
            var finalArgs = args.Replace("$IMAGE", file.Name);

            var output = await SpawnProcess(finalArgs);

            var report = $"({counter}/{total}) {output}";

            var percentage = CalculatePercentage(counter, total);
            
            yield return (report, percentage);
        }
    }

    private double CalculatePercentage(int counter, int total) => (double)(counter * 100) / total;

    private async Task<string> SpawnProcess(string args)
    {
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

        var result = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        return result;
    }

    private string GetWaifu()
    {
        var file = _directoryService
            .GetWaifuDirectory()
            .EnumerateFiles()
            .FirstOrDefault(x => x.Name is "waifu2x-ncnn-vulkan");

        return file?.FullName ?? throw new FileNotFoundException("Waifu2x executable not found!");
    }
}