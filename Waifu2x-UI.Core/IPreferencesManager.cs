namespace Waifu2x_UI.Core;

public interface IPreferencesManager
{
    Task SavePreferences(Command command);
    Task<Command?> LoadPreferences();
}