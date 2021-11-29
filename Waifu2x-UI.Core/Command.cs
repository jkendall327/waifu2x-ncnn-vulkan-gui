using System.Text;

namespace Waifu2x_UI.Core;

public class Command
{
    public Command(string inputImagePath, string outputImagePath)
    {
        InputImagePath = inputImagePath;
        OutputImagePath = outputImagePath;
    }

    public Command()
    {
        
    }

    public string GetArguments()
    {
        var command = new StringBuilder();

        command.Append($" -input-path {InputImagePath}");
        command.Append($" -output-path {GetOutput()}");

        command.Append($" -format {OutputFileType.ToExtension()}");

        if (Verbose)
        {
            command.Append(" -verbose");
        }

        return command.ToString();
    }

    private string GetOutput() => $"{OutputImagePath}{Suffix ?? string.Empty}.{OutputFileType.ToExtension()}";

    public string? Suffix { get; set; }
    public OutputFileType OutputFileType { get; set; } = OutputFileType.Png;
    public bool Verbose { get; set; } = true;
    public string? InputImagePath { get; set; }
    public string? OutputImagePath { get; set; }
}