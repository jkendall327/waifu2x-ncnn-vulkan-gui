using System.Collections.Generic;
using System.IO;
using System.IO.Abstractions;
using System.Reactive;
using ReactiveUI;
using Waifu2x_UI.Core;

namespace Waifu2x_UI.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        // Services
        private readonly ICommandRunner _runner;
        
        // Viewmodels
        public FilePickerViewModel InputImagePicker { get; }
        public FilePickerViewModel OutputImagePicker { get; }

        // Commands and interactions
        public ReactiveCommand<Unit, Unit> RunCommand { get; }
        public Interaction<Unit, string[]> FindImageDialog { get; } = new();
        
        // Models
        public Command Command { get; }= new();
        
        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;

            InputImagePicker = new("Select input...", FindImageDialog);
            OutputImagePicker = new("Set output directory...", FindImageDialog);
            
            RunCommand = CreateRunCommand();

            LinkInputToCommand();
        }

        private void LinkInputToCommand()
        {
            var observer = Observer.Create<List<FileInfo>>(input => { Command.InputImages = input; });

            this.WhenAnyValue(viewModel => viewModel.InputImagePicker.Files).Subscribe(observer);
        }

        private ReactiveCommand<Unit, Unit> CreateRunCommand()
        {
            bool Selector(string input, string output)
            {
                return !string.IsNullOrEmpty(input) && !string.IsNullOrEmpty(output);
            }

            var canExecute = this.WhenAnyValue(
                vm => vm.InputImagePicker.Content,
                vm => vm.OutputImagePicker.Content,
                Selector);

            return ReactiveCommand.Create(Run, canExecute);
        }

        private void Run()
        {
            var output = _runner.Run(Command);
        }
    }
}