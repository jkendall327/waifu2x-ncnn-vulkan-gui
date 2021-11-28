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

        public FilePickerViewModel ExecutablePicker { get; }
        public FilePickerViewModel InputImagePicker { get; }
        
        [Reactive] public string CurrentCommand { get; set; } = string.Empty;
        [Reactive] public bool VerboseOutput { get; set; }
        
        public ReactiveCommand<Unit, Unit> RunCommandCommand { get; }
        public Interaction<Unit, string?> OpenFindExecutableDialog { get; } = new();
        public Interaction<Unit, string?> OpenFindInputImageDialog { get; } = new();

        public MainWindowViewModel(ICommandRunner runner)
        {
            _runner = runner;
            
            var canExecute = StringHasValue(x => x.CurrentCommand);
            
            RunCommandCommand = ReactiveCommand.Create(RunCommand, canExecute);
            
            ExecutablePicker = new("Please select your Waifu2x executable", OpenFindExecutableDialog);
            InputImagePicker = new("Please select your input image", OpenFindInputImageDialog);
        }

        private IObservable<bool> StringHasValue(Expression<Func<MainWindowViewModel,string>> expression)
        {
            return this.WhenAnyValue(expression, str => !string.IsNullOrEmpty(str));
        }

        private void RunCommand()
        {
            var command = new Command();

            var output = _runner.Run(ExecutablePicker.Content, command);
        }
    }
}