﻿using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.IO.Abstractions;
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
    private readonly IDirectory _directory;
        
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
    
    public List<int> DenoiseLevels { get; set; }= new() { 0, 1, 2, 3 };
    public List<int> ScaleFactors { get; set; }= new() { 2, 4, 8, 16 };
    public List<string> Models { get; set; }

    // Models
    public Command Command { get; }
        
    public MainWindowViewModel(ICommandRunner runner, IDirectory directory, Command? command = null)
    {
        _runner = runner;
        _directory = directory;

        Command = command ?? new Command(directory);

        Models = GetModels();
        
        InputImagePicker = new("Select input...", FindImageDialog);
        OutputDirectoryPicker = new("Set output directory...", FindOutputDirectoryDialog);
            
        RunCommand = CreateRunCommand();

        InputImagePicker.Files.CollectionChanged += FilesOnCollectionChanged;
        LinkOutputToCommand();
    }

    private List<string> GetModels()
    {
        return _directory.GetWaifuDirectory()
            .EnumerateDirectories()
            .Select(x => x.Name)
            .ToList();
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
            return !string.IsNullOrEmpty(input) && _directory.Exists(output?.FullName);
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

        // await _preferencesManager.SavePreferences(Command);
        
        await foreach (var (report, progress) in _runner.Run(Command))
        {
            Report += report;
            Progress = progress;
        }
    }
}