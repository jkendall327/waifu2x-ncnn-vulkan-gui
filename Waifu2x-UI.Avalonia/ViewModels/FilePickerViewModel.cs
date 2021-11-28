using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2x_UI.Avalonia.ViewModels;

public class FilePickerViewModel : ViewModelBase
{
    [Reactive] public string Watermark { get; set; }
    [Reactive] public string Content { get; set; } = string.Empty;

    public List<FileInfo> Files { get; set; } = new();
    
    private ReactiveCommand<Unit, Unit> OpenDialogCommand { get; }

    public FilePickerViewModel(string watermark, Interaction<Unit, string[]> dialogInteraction)
    {
        Watermark = watermark;
        
        OpenDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            var result = await dialogInteraction.Handle(Unit.Default);

            Files = result.Select(x => new FileInfo(x)).ToList();
            
            Content = string.Join(", ", Files.Select(x => x.Name));
        });
    }
}