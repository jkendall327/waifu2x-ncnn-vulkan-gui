using System;
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
        [Reactive] public string Suffix { get; set; } = "-upscaled";
        [Reactive] public bool Verbose { get; set; } = true;
        
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public Interaction<Unit, string[]> FindImageDialog { get; } = new();

        /// <summary>
        /// If this is false, then use JPG
        /// </summary>
        [Reactive]
        public bool PngOutput { get; set; } = true;

        private readonly Command _command = new();
        
        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            RunCommand = CreateRunCommand();
            
            InputImagePicker = new("Select input...", FindImageDialog);
            OutputImagePicker = new("Set output directory...", FindImageDialog);
        }

        private void SetUpCommandPreview()
        {
            throw new NotImplementedException();
            
            var command = new Command(string.Empty, string.Empty);
            
            // set this up to update preview bar when anything changes...
            
            var changes = this.WhenAnyValue(
                x => x.InputImagePicker.Content,
                u => u.OutputImagePicker.Content,
                z => z.Verbose,
                ((s, s1, arg3) => command.GetArguments()));
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
            _command.InputImagePath = InputImagePicker.Content;
            _command.OutputImagePath = OutputImagePicker.Content;
            
            _command.Verbose = Verbose;
            _command.OutputFileType = PngOutput ? OutputFileType.Png : OutputFileType.Jpeg;

            var output = _runner.Run(_command);
        }
    }
}