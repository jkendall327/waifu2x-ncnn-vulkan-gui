using System;
using System.IO;
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
            
            this.WhenActivated(
                d => d(ViewModel!.OpenFindExecutableDialog.RegisterHandler(ShowDialogAsync)));
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

            var result = await dialog.ShowAsync(this) ?? Array.Empty<string>();
            
            interaction.SetOutput(result.FirstOrDefault());
        }
    }
}