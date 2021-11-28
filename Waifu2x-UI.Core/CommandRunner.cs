using System.Diagnostics;
using System.Text;

namespace Waifu2x_UI.Core;

public class CommandRunner : ICommandRunner
{
    private readonly string _shellPath = "/bin/bash";

    public CommandRunner(string? shellPath = null)
    {
        if (shellPath is null) return;
        _shellPath = shellPath;
    }

    public string Run(string waifu2XPath, Command command)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = waifu2XPath,
            Arguments = command.GetArguments(),
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
}