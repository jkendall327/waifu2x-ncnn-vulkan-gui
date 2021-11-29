using System.Collections.Specialized;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Waifu2x_UI.Core;

namespace Waifu2xUI.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // Services
    private readonly ICommandRunner _runner;
    private readonly IPreferencesManager _preferencesManager;
        
    // Viewmodels
    public FilePickerViewModel InputImagePicker { get; }
    public DirectoryPickerViewModel OutputDirectoryPicker { get; }

    // Commands and interactions
    public ReactiveCommand<Unit, Unit> RunCommand { get; }
    public Interaction<Unit, string[]> FindImageDialog { get; } = new();
    public Interaction<Unit, DirectoryInfo> FindOutputDirectoryDialog { get; } = new();
        
    // Bindings
    [Reactive] public string Report { get; set; } = string.Empty;
    
    [Reactive] public double Progress { get; set; }
    
    // Models
    public Command Command { get; }
        
    public MainWindowViewModel(ICommandRunner runner, IPreferencesManager preferencesManager, Command? command = null)
    {
        _runner = runner;
        _preferencesManager = preferencesManager;

        Command = command ?? new Command();

        InputImagePicker = new("Select input...", FindImageDialog);
        OutputDirectoryPicker = new("Set output directory...", FindOutputDirectoryDialog);
            
        RunCommand = CreateRunCommand();

        InputImagePicker.Files.CollectionChanged += FilesOnCollectionChanged;
        LinkOutputToCommand();
    }

    private void FilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Command.InputImages = InputImagePicker.Files.ToList();
    }

    private void LinkOutputToCommand()
    {
        var observer = Observer.Create<DirectoryInfo?>(input =>
        {
            if (input is null) return;
            Command.OutputDirectory = input;
        });

        this.WhenAnyValue(viewModel => viewModel.OutputDirectoryPicker.Directory).Subscribe(observer);
    }

    private ReactiveCommand<Unit, Unit> CreateRunCommand()
    {
        bool Selector(string input, DirectoryInfo? output)
        {
            return !string.IsNullOrEmpty(input) && Directory.Exists(output?.FullName);
        }

        var canExecute = this.WhenAnyValue(
            vm => vm.InputImagePicker.Content,
            vm => vm.OutputDirectoryPicker.Directory,
            Selector);

        return ReactiveCommand.CreateFromTask(Run, canExecute);
    }
    
    private async Task Run()
    {
        Report = string.Empty;
        Progress = 0;

        await _preferencesManager.SavePreferences(Command);
        
        await foreach (var (report, progress) in _runner.Run(Command))
        {
            Report += report;
            Progress = progress;
        }
    }
}