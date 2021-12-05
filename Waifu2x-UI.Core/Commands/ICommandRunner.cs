namespace Waifu2x_UI.Core.Commands;

public interface ICommandRunner
{
    IAsyncEnumerable<(string, double)> Run(Command command);
}