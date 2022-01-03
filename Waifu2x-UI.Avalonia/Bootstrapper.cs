using System;
using System.IO;
using System.IO.Abstractions;
using System.Text.Json;
using Avalonia.Controls;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu2x_UI.Core.Commands;
using Waifu2x_UI.Core.Filesystem;
using Waifu2x_UI.Core.Serialization;
using Waifu2xUI.Avalonia.ViewModels;
using Waifu2xUI.Avalonia.Views;

namespace Waifu2xUI.Avalonia;

public class Bootstrapper
{
    public Window Setup()
    {
        var provider = BuildServiceProvider();
        return provider.GetRequiredService<MainWindow>();
    }

    private IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        var filesystem = new FileSystem();

        filesystem.Directory.CreateDirectory(Locations.AppConfigFolder);

        services.AddSingleton(filesystem.Directory);
        services.AddSingleton(filesystem.DirectoryInfo);
        services.AddSingleton(filesystem.FileInfo);
        services.AddSingleton(filesystem.File);

        services.AddTransient<ICommandRunner, CommandRunner>();
        services.AddTransient<IDirectoryService, DirectoryService>();
        services.AddTransient<IPreferencesManager, PreferencesManager>();
        services.AddTransient<MainWindowViewModel>();

        var options = SetupConfiguration(filesystem.File);

        services.AddSingleton(options);

        SetupLogging(services);

        services.AddTransient(s => new MainWindow
        {
            DirectoryService = s.GetRequiredService<IDirectoryService>(),
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });

        return services.BuildServiceProvider();
    }

    private void SetupLogging(IServiceCollection services)
    {
        services.AddLogging(builder =>
        {
            var logger = new LoggerConfiguration()
                .WriteTo.File(Path.Combine(Locations.AppConfigFolder, "app.log"))
                .CreateLogger();

            builder.AddConsole();
            builder.AddSerilog(logger);
        });
    }

    private SerializationOptions SetupConfiguration(IFile file)
    {
        var configPath = Path.Combine(Locations.AppConfigFolder, "appsettings.json");

        if (!file.Exists(configPath))
        {
            var newOptions = new SerializationOptions();
            var json = JsonSerializer.Serialize(newOptions);
            file.WriteAllText(configPath, json);
        }

        var config = new ConfigurationBuilder()
            .AddJsonFile(configPath)
            .Build();

        SerializationOptions options = new();

        config.GetSection(nameof(SerializationOptions)).Bind(options);
        return options;
    }
}