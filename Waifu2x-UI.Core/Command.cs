namespace Waifu2x_UI.Core;

public class Command
{
    private readonly string _waifu2XPath;

    public Command(string waifu2XPath)
    {
        _waifu2XPath = waifu2XPath;
    }

    internal string GetArguments()
    {
        var cmd = _waifu2XPath;
        
        // query flags...
        
        var escapedArgs = cmd.Replace("\"", "\\\"");

        return $"-c \"{escapedArgs}\"";
    }

    public string InputImagePath { get; set; }
    public string OutputImagePath { get; set; }
    public Flags Flags { get; } = new();
}