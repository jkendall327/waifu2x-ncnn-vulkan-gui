namespace Waifu2x_UI.Core;

public class Command
{
    internal string GetArguments()
    {
        var cmd = string.Empty;
        // query flags...
        
        var escapedArgs = cmd.Replace("\"", "\\\"");

        return $"-c \"{escapedArgs}\"";
    }

    public bool Verbose { get; set; }
    public string InputImagePath { get; set; }
    public string OutputImagePath { get; set; }
    public Flags Flags { get; } = new();
}