namespace Waifu2x_UI.Core;

public interface ICommandRunner
{
    IEnumerable<string> Run(Command command);
}