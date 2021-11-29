using System;
using System.IO;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Waifu2x_UI.Core;
using Waifu2xUI.Avalonia.ViewModels;

namespace Waifu2xUI.Avalonia.Views;

public class MainWindow : ReactiveWindow<MainWindowViewModel>
{
    public MainWindow()
    {
        InitializeComponent();
#if DEBUG
        this.AttachDevTools();
#endif
            
        this.WhenActivated(d =>
        {
            d(ViewModel!.FindImageDialog.RegisterHandler(ShowImageDialogAsync));
            d(ViewModel!.FindOutputDirectoryDialog.RegisterHandler(ShowSelectDirectoryDialogAsync));
        });
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private async Task ShowSelectDirectoryDialogAsync(InteractionContext<Unit, DirectoryInfo> interaction)
    {
        var dialog = new OpenFolderDialog
        {
            Directory = Directory.GetCurrentDirectory(),
            Title = "Select output directory"
        };

        var result = await dialog.ShowAsync(this);

        if (result is null)
        {
            interaction.SetOutput(DirectoryExtensions.GetOutputDirectory());
            return;
        }
            
        interaction.SetOutput(new(result));
    }
        
    private async Task ShowImageDialogAsync(InteractionContext<Unit, string[]> interaction)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = true,
                
            Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop),
                
            Filters = new()
            {
                new()
                {
                    Name = "Images",
                    Extensions = new(){"jpg", "jpeg", "png", "webp"}
                },
                new()
                {
                    Name = "All files",
                    Extensions = new(){"*"}
                }
            }
        };

        var result = await dialog.ShowAsync(this);
            
        interaction.SetOutput(result ?? Array.Empty<string>());
    }
}