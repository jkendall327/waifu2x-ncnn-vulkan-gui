using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Waifu2x_UI.Core;

namespace Waifu2x_UI.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ICommandRunner _runner;
        public FilePickerViewModel InputImagePicker { get; }
        public FilePickerViewModel OutputImagePicker { get; }
        
        [Reactive] public string Command { get; set; } = string.Empty;
        [Reactive] public bool Verbose { get; set; }
        
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public Interaction<Unit, string?> FindImageDialog { get; } = new();
        
        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            RunCommand = CreateRunCommand();
            
            InputImagePicker = new("Please select your input image", FindImageDialog);
            OutputImagePicker = new("Please select where the output will be placed", FindImageDialog);
        }

        private ReactiveCommand<Unit, Unit> CreateRunCommand()
        {
            var canExecute = this.WhenAnyValue(
                vm => vm.InputImagePicker.Content,
                vm => vm.OutputImagePicker.Content,
                (input, output) =>
                {
                    return !string.IsNullOrEmpty(input) &&
                           !string.IsNullOrEmpty(output);
                });

            return ReactiveCommand.Create(Run);
        }

        private void Run()
        {
            var command = new Command(InputImagePicker.Content, OutputImagePicker.Content)
            {
                Verbose = Verbose
            };

            var output = _runner.Run(command);
        }
    }
}