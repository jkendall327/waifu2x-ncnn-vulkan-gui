using System.IO.Abstractions;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Waifu2x_UI.Core.Commands;

namespace Waifu2x_UI.Core.Serialization;

public class PreferencesManager : IPreferencesManager
{
    private readonly IFile _file;

    private readonly string _filepath = "userpreferences.json";

    private readonly SerializationOptions _options;
    private readonly ILogger<PreferencesManager> _logger;

    public PreferencesManager(IFile file, SerializationOptions options, ILogger<PreferencesManager> logger)
    {
        _file = file;
        _options = options;
        _logger = logger;
    }

    public async Task SavePreferences(Command command)
    {
        if (!_options.SerializationEnabled) return;

        _logger.LogInformation("Saving user data");

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
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Exception occured while deserializing data");
            return null;
        }
    }
}