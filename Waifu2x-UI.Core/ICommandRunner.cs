namespace Waifu2x_UI.Core;

public interface ICommandRunner
{
    IAsyncEnumerable<string> Run(Command command);
}