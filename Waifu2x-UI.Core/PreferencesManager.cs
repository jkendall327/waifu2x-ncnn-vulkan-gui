using System.IO.Abstractions;
using System.Text.Json;

namespace Waifu2x_UI.Core;

public class PreferencesManager : IPreferencesManager
{
    private readonly IFile _file;

    private readonly string _filepath = "userpreferences.json";

    private readonly SerializationOptions _options;
    
    public PreferencesManager(IFile file, SerializationOptions options)
    {
        _file = file;
        _options = options;
    }

    public async Task SavePreferences(Command command)
    {
        if (!_options.SerializationEnabled) return;
        
        await using var stream = _file.Create(_filepath);

        var options = _options.PrettyPrint ? new JsonSerializerOptions { WriteIndented = true } : null;
        
        await JsonSerializer.SerializeAsync(stream, command, options);
    }

    public async Task<Command?> LoadPreferencesAsync()
    {
        if (!_options.SerializationEnabled) return null;

        if (!_file.Exists(_filepath)) return null;

        await using var stream = new FileStream(_filepath, FileMode.Open);
        
        return await JsonSerializer.DeserializeAsync<Command>(stream);
    }
    
    public Command? LoadPreferences()
    {
        if (!_options.SerializationEnabled) return null;

        if (!_file.Exists(_filepath)) return null;

        try
        {
            return JsonSerializer.Deserialize<Command>(_file.ReadAllText(_filepath));
        }
        catch (JsonException)
        {
            return null;
        }
    }
}