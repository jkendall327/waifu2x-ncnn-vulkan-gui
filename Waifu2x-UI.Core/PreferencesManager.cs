using System.IO.Abstractions;
using System.Text.Json;

namespace Waifu2x_UI.Core;

public class PreferencesManager : IPreferencesManager
{
    private readonly IFile _file;

    private readonly string _filepath = "userpreferences.json";

    public PreferencesManager(IFile file) => _file = file;

    public async Task SavePreferences(Command command)
    {
        await using var stream = _file.Create(_filepath);
        
        await JsonSerializer.SerializeAsync(stream, command);
    }

    public async Task<Command?> LoadPreferences()
    {
        if (!_file.Exists(_filepath)) return null;

        await using var stream = new FileStream(_filepath, FileMode.Open);
        
        return await JsonSerializer.DeserializeAsync<Command>(stream);
    }
}