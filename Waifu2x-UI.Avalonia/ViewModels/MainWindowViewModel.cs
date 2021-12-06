using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using Waifu2x_UI.Core;
using Waifu2x_UI.Core.Commands;
using Waifu2x_UI.Core.Filesystem;
using Waifu2x_UI.Core.Serialization;

namespace Waifu2xUI.Avalonia.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    // Services
    private readonly ICommandRunner _runner;
    private readonly IDirectoryService _directoryService;
    private readonly IPreferencesManager _preferencesManager;
    private readonly ILogger<MainWindowViewModel> _logger;

    // Viewmodels
    public FilePickerViewModel InputImagePicker { get; }
    public DirectoryPickerViewModel OutputDirectoryPicker { get; }

    // Commands and interactions
    public ReactiveCommand<Unit, Unit> RunCommand { get; }
    public Interaction<Unit, string[]> FindImageDialog { get; } = new();
    public Interaction<Unit, IDirectoryInfo> FindOutputDirectoryDialog { get; } = new();

    // Bindings
    [Reactive] public string Report { get; set; } = string.Empty;

    [Reactive] public double Progress { get; set; }

    public List<int> DenoiseLevels { get; set; } = new() { 0, 1, 2, 3 };
    public List<int> ScaleFactors { get; set; } = new() { 2, 4, 8, 16 };
    public List<string> Models { get; set; }

    // Models
    public Command Command { get; }

    public MainWindowViewModel(ICommandRunner runner, IFileInfoFactory factory, IDirectoryService directoryService,
        IPreferencesManager preferencesManager, ILogger<MainWindowViewModel> logger)
    {
        _runner = runner;
        _directoryService = directoryService;
        _preferencesManager = preferencesManager;
        _logger = logger;

        logger.LogInformation("App started");

        Command = _preferencesManager.LoadPreferences() ?? new Command();

        Models = GetModels();

        InputImagePicker = new("Select input...", Command, FindImageDialog, factory);
        OutputDirectoryPicker = new("Set output directory...", FindOutputDirectoryDialog);

        RunCommand = CreateRunCommand();

        InputImagePicker.Files.CollectionChanged += FilesOnCollectionChanged;
        LinkOutputToCommand();
    }

    private List<string> GetModels()
    {
        var directory = _directoryService.GetWaifuDirectory();

        var models = directory
            .EnumerateDirectories()
            .Select(x => x.Name)
            .ToList();

        _logger.LogInformation("Loaded {ModelCount} upscaling models from {ModelDirectory}",
            models.Count, directory.FullName);

        return models;
    }

    private void FilesOnCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        Command.InputImages = InputImagePicker.Files.ToList();
    }

    private void LinkOutputToCommand()
    {
        var observer = Observer.Create<IDirectoryInfo?>(input =>
        {
            if (input is null) return;

            _logger.LogInformation("Output directory changed to {DirectoryName}", input.FullName);
            Command.OutputDirectory = input;
        });

        this.WhenAnyValue(viewModel => viewModel.OutputDirectoryPicker.Directory).Subscribe(observer);
    }

    private ReactiveCommand<Unit, Unit> CreateRunCommand()
    {
        bool Selector(string input, IDirectoryInfo? output)
        {
            return !string.IsNullOrEmpty(input) && _directoryService.Exists(output);
        }

        var canExecute = this.WhenAnyValue(
            vm => vm.InputImagePicker.Content,
            vm => vm.OutputDirectoryPicker.Directory,
            Selector);

        return ReactiveCommand.CreateFromTask(Run, canExecute);
    }

    private async Task Run()
    {
        _logger.LogInformation("Running command: {Preview}", Command.Preview);

        await _preferencesManager.SavePreferences(Command);

        Report = string.Empty;
        Progress = 0;

        await foreach (var (report, progress) in _runner.Run(Command))
        {
            Report += report;
            Progress = progress;
        }
    }
}