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
            FileName = _shellPath,
            Arguments = GenerateArguments(waifu2XPath, command),
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

    private string GenerateArguments(string waifu2XPath, Command command)
    {
        var sb = new StringBuilder();

        sb.Append(waifu2XPath);

        sb.Append(command.GetArguments());

        return sb.ToString();
    }
}