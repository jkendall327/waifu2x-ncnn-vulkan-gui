using System.Reactive;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Waifu2x_UI.Core;

namespace Waifu2x_UI.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ICommandRunner _runner;

        public FilePickerViewModel ExecutablePicker { get; }
        public FilePickerViewModel InputImagePicker { get; }
        public FilePickerViewModel OutputImagePicker { get; }
        
        [Reactive] public string Command { get; set; } = string.Empty;
        [Reactive] public bool Verbose { get; set; }
        
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public Interaction<Unit, string?> OpenFindExecutableDialog { get; } = new();
        public Interaction<Unit, string?> FindImageDialog { get; } = new();
        
        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            RunCommand = CreateRunCommand();
            
            ExecutablePicker = new("Please select your Waifu2x executable", OpenFindExecutableDialog);
            InputImagePicker = new("Please select your input image", FindImageDialog);
            OutputImagePicker = new("Please select where the output will be placed", FindImageDialog);
        }

        private ReactiveCommand<Unit, Unit> CreateRunCommand()
        {
            var canExecute = this.WhenAnyValue(
                viewModel => viewModel.Command, 
                command => !string.IsNullOrEmpty(command));

            return ReactiveCommand.Create(Run, canExecute);
        }

        private void Run()
        {
            var command = new Command
            {
                InputImagePath = InputImagePicker.Content,
                OutputImagePath = OutputImagePicker.Content,
                Verbose = Verbose
            };

            var output = _runner.Run(ExecutablePicker.Content, command);
        }
    }
}