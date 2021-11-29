using System;
using System.IO.Abstractions;
using System.Text.Json;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
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
        var file = new FileSystem().File;
        
        var runner = new CommandRunner();
        var manager = new PreferencesManager(file);

        //var userData = await LoadUserData(manager);

        Command? userData = null;
        
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            desktop.MainWindow = new MainWindow
            {
                DataContext = new MainWindowViewModel(runner, manager, userData)
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private static async Task<Command?> LoadUserData(IPreferencesManager manager)
    {
        try
        {
            return await manager.LoadPreferences();
        }
        catch (JsonException)
        {
            return null;
        }
    }
}