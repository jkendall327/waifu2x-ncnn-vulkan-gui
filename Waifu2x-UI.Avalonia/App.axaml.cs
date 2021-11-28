using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using Waifu2x_UI.Avalonia.ViewModels;
using Waifu2x_UI.Avalonia.Views;
using Waifu2x_UI.Core;

namespace Waifu2x_UI.Avalonia
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }

        public override void OnFrameworkInitializationCompleted()
        {
            var runner = new CommandRunner();
            
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.MainWindow = new MainWindow
                {
                    DataContext = new MainWindowViewModel(runner),
                };
            }

            base.OnFrameworkInitializationCompleted();
        }
    }
}