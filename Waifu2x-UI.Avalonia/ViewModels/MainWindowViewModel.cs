﻿using System.Reactive;
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

            this.WhenAnyValue(
                    x => x.InputImagePicker.Content,
                    y => y.OutputImagePicker.Content,
                    (x, y) => (x, y))
                .Subscribe(Observer.Create<(string, string)>(tuple =>
                {
                    var (input, output) = tuple;
                    
                    Command.InputImagePath = input;
                    Command.OutputImagePath = output;
                }));
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