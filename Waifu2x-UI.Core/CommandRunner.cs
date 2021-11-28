using System.Diagnostics;

namespace Waifu2x_UI.Core;

public class CommandRunner
{
    private readonly string _executableFilepath = "/bin/bash";

    public CommandRunner(string? executableFilepath = null)
    {
        if (executableFilepath is null) return;
        _executableFilepath = executableFilepath;
    }

    public string Run(Command command)
    {
        var processStartInfo = new ProcessStartInfo
        {
            FileName = _executableFilepath,
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