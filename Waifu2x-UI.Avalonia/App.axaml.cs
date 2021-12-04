using System;
using System.IO.Abstractions;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using Waifu2x_UI.Core;
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

        services.AddTransient(s => new MainWindow
        {
            DirectoryService = s.GetRequiredService<IDirectoryService>(),
            DataContext = s.GetRequiredService<MainWindowViewModel>()
        });

        return services.BuildServiceProvider();
    }
}