using System.Diagnostics;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    public string Run(Command command)
    {
        var waifuPath = GetWaifu();
        
        var processStartInfo = new ProcessStartInfo
        {
            FileName = waifuPath,
            Arguments = command.GetPreview(),
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

        var waifu = new DirectoryInfo(parent.FullName + "waifu2x");
        
        var file = waifu.EnumerateFiles().FirstOrDefault(x => x.Name is "waifu2x-ncnn-vulkan");

        return file?.FullName ?? throw new FileNotFoundException("Waifu2x executable not found!");
    }
}