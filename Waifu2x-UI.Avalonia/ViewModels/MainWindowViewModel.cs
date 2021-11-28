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
        
        [Reactive] public string CurrentCommand { get; set; }
        [Reactive] public string ExecutableFilepath { get; set; }
        [Reactive] public string SelectExecutablePrompt { get; set; }
        public ReactiveCommand<Unit, Unit> RunCommandCommand { get; }
        public ReactiveCommand<Unit, Unit> PickExecutableCommand { get; }
        public Interaction<Unit, string?> OpenFindExecutableDialog { get; } = new();

        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            SetupExecutablePrompt();

            var canExecute = StringHasValue(x => x.CurrentCommand);
            
            RunCommandCommand = ReactiveCommand.Create(RunCommand, canExecute);
            PickExecutableCommand = ReactiveCommand.CreateFromTask(PickExecutable);
        }

        private IObservable<bool> GetCanExecute()
        {
            return this.WhenAnyValue(
                viewModel => viewModel.CurrentCommand,
                currentCommand => !string.IsNullOrEmpty(currentCommand));
        }

        private void SetupExecutablePrompt()
        {
            void OnNext(bool isEmpty)
            {
                SelectExecutablePrompt = isEmpty ? string.Empty : "Please select your Waifu2X executable file.";
            }

            StringHasValue(x => x.ExecutableFilepath)
                .Subscribe(
                    OnNext,
                    exception => throw exception,
                    () => throw new InvalidOperationException());
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
    }
}