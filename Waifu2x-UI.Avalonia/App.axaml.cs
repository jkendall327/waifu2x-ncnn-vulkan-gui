using System;
using System.IO;
using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Waifu2x_UI.Core;
using Waifu2x_UI.Core.Commands;
using Waifu2x_UI.Core.Filesystem;
using Waifu2x_UI.Core.Serialization;
using Waifu2xUI.Avalonia.ViewModels;
using Waifu2xUI.Avalonia.Views;

namespace Waifu2xUI.Avalonia;

public class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var provider = BuildServiceProvider();
        
        var mainWindow = provider.GetRequiredService<MainWindow>();
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = mainWindow;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static IServiceProvider BuildServiceProvider()
    {
        var services = new ServiceCollection();

        var filesystem = new FileSystem();

        services.AddSingleton(filesystem.Directory);
        services.AddSingleton(filesystem.DirectoryInfo);
        services.AddSingleton(filesystem.FileInfo);
        services.AddSingleton(filesystem.File);
        
        services.AddTransient<ICommandRunner, CommandRunner>();
        services.AddTransient<IDirectoryService, DirectoryService>();
        services.AddTransient<IPreferencesManager, PreferencesManager>();
        services.AddTransient<MainWindowViewModel>();
        
        var options = SetupConfiguration();

        services.AddSingleton(options);

        SetupLogging(services);

        services.AddTransient(s => new MainWindow
        {
            DirectoryService = s.GetRequiredService<IDirectoryService>(),
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });

        return services.BuildServiceProvider();
    }

    private static void SetupLogging(IServiceCollection services)
    {
        var logger = new LoggerConfiguration()
            .WriteTo.File("app.log")
            .CreateLogger();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
            builder.AddSerilog(logger);
        });
    }

    private static SerializationOptions SetupConfiguration()
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        SerializationOptions options = new();

        config.GetSection(nameof(SerializationOptions)).Bind(options);
        return options;
    }
}