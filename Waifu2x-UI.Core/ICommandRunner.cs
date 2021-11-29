namespace Waifu2x_UI.Core;

public interface ICommandRunner
{
    List<string> Run(Command command);
}