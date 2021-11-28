using System;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.ReactiveUI;
using ReactiveUI;
using Waifu2x_UI.Avalonia.ViewModels;

namespace Waifu2x_UI.Avalonia.Views
{
    public partial class MainWindow : ReactiveWindow<MainWindowViewModel>
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
                });
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
        
        private async Task ShowDialogAsync(InteractionContext<Unit, string?> interaction)
        {
            var dialog = new OpenFileDialog
            {
                AllowMultiple = false,
                Directory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
            };

            var result = await dialog.ShowAsync(this);
            
            interaction.SetOutput(result?.FirstOrDefault());
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
}