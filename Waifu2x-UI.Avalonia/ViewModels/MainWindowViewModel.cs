using System;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Waifu2x_UI.Core;

namespace Waifu2x_UI.Avalonia.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        private readonly ICommandRunner _runner;

        [Reactive] public string CurrentCommand { get; set; } = string.Empty;
        [Reactive] public string ExecutableFilepath { get; set; } = string.Empty;
        [Reactive] public string InputImagePath { get; set; } = string.Empty;
        [Reactive] public bool VerboseOutput { get; set; }
        
        public ReactiveCommand<Unit, Unit> RunCommandCommand { get; }
        public ReactiveCommand<Unit, Unit> PickExecutableCommand { get; }
        public Interaction<Unit, string?> OpenFindExecutableDialog { get; } = new();
        
        public ReactiveCommand<Unit, Unit> PickInputImageCommand { get; }
        public Interaction<Unit, string?> OpenFindInputImageDialog { get; } = new();

        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            var canExecute = StringHasValue(x => x.CurrentCommand);

            PickExecutableCommand = ReactiveCommand.CreateFromTask(PickExecutable);
            PickInputImageCommand = ReactiveCommand.CreateFromTask(PickInputImage);

            RunCommandCommand = ReactiveCommand.Create(RunCommand, canExecute);
        }

        private IObservable<bool> StringHasValue(Expression<Func<MainWindowViewModel,string>> expression)
        {
            return this.WhenAnyValue(expression, str => !string.IsNullOrEmpty(str));
        }

        private void RunCommand()
        {
            var command = new Command();

            var output = _runner.Run(ExecutableFilepath, command);
        }

        private async Task PickExecutable()
        {
            var result = await OpenFindExecutableDialog.Handle(Unit.Default);

            ExecutableFilepath = result ?? string.Empty;
        }
        
        private async Task PickInputImage()
        {
            var result = await OpenFindInputImageDialog.Handle(Unit.Default);

            InputImagePath = result ?? string.Empty;
        }
    }
}