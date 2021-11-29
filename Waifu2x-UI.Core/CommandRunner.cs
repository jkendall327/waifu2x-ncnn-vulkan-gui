using System.Diagnostics;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    private readonly string _waifuPath;

    public CommandRunner()
    {
        _waifuPath = GetWaifu();
    }
    
    public async IAsyncEnumerable<string> Run(Command command)
    {
        var files = command.InputImages;

        foreach (var file in files)
        {
            var output = await SpawnProcess(command, file);
            
            yield return output;
        }
    }

    private async Task<string> SpawnProcess(Command command, FileInfo file)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = _waifuPath,
            Arguments = command.GenerateArguments(file),
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
        var parent = Directory.GetParent(Directory.GetCurrentDirectory()) ?? throw new DirectoryNotFoundException();

        var waifu = new DirectoryInfo(parent.FullName + Path.DirectorySeparatorChar + "waifu2x");
        
        var file = waifu.EnumerateFiles().FirstOrDefault(x => x.Name is "waifu2x-ncnn-vulkan");

        return file?.FullName ?? throw new FileNotFoundException("Waifu2x executable not found!");
    }
}