using System.Collections.ObjectModel;
using System.IO;
using System.IO.Abstractions;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using Waifu2x_UI.Core;

namespace Waifu2xUI.Avalonia.ViewModels;

public class FilePickerViewModel : PickerBaseViewModel
{
    public ObservableCollection<IFileInfo> Files { get; } = new();
    
    private ReactiveCommand<Unit, Unit> OpenDialogCommand { get; }

    public FilePickerViewModel(string watermark, Command command, Interaction<Unit, string[]> dialogInteraction, IFileInfoFactory fileInfoFactory) : base(watermark)
    {
        OpenDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await dialogInteraction.Handle(Unit.Default);

            var files = result.Select(x => fileInfoFactory.FromFileName(x)).ToList();
            
            Files.Clear();

            foreach (var file in files)
            {
                Files.Add(file);
            }

            if (!files.Any()) return;

            command.OutputDirectory ??= files.First().Directory;

            if (files.Count is 1)
            {
                Content = files.First().FullName;
                return;
            }
            
            Content = string.Join(", ", Files.Select(x => x.Name));
        });
    }
}