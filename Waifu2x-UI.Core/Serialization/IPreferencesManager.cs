using Waifu2x_UI.Core.Commands;

namespace Waifu2x_UI.Core.Serialization;

public interface IPreferencesManager
{
    Task SavePreferences(Command command);
    Task<Command?> LoadPreferencesAsync();
    Command? LoadPreferences();
}