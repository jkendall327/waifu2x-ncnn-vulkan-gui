namespace Waifu2x_UI.Core;

public interface ICommandRunner
{
    string Run(string waifu2XPath, Command command);
}