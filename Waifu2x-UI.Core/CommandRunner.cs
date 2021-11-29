using System.Diagnostics;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    private readonly string _waifuPath;

    public CommandRunner()
    {
        _waifuPath = GetWaifu();
    }
    
    public List<string> Run(Command command)
    {
        var files = command.InputImages;

        var output = files.Select(file => SpawnProcess(command, file)).ToList();

        return output;
    }

    private string SpawnProcess(Command command, FileInfo file)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = _waifuPath,
            Arguments = command.GenerateArguments(file),
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true,
        };

        var process = new Process
        {
            StartInfo = processStartInfo
        };

        process.Start();

        var result = process.StandardOutput.ReadToEnd();

        process.WaitForExit();

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