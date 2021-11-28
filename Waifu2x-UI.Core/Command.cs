using System.Text;

namespace Waifu2x_UI.Core;

public class Command
{
    public Command(string inputImagePath, string outputImagePath)
    {
        InputImagePath = inputImagePath;
        OutputImagePath = outputImagePath;
    }

    internal string GetArguments()
    {
        var command = new StringBuilder();

        command.Append($" -i {InputImagePath}");
        command.Append($" -o {OutputImagePath}");
        
        //var escapedArgs = cmd.Replace("\"", "\\\"");
        //return $"-c \"{escapedArgs}\"";

        return command.ToString();
    }

    public bool Verbose { get; set; }
    private string InputImagePath { get; }
    private string OutputImagePath { get; }
}