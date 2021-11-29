using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2x_UI.Avalonia.ViewModels;

public class DirectoryPickerViewModel : ViewModelBase
{
    [Reactive] public string Watermark { get; set; }    
    [Reactive] public string Content { get; set; } = string.Empty;
    [Reactive] public DirectoryInfo? Directory { get; set; }
    
    private ReactiveCommand<Unit, Unit> OpenDialogCommand { get; }

    public DirectoryPickerViewModel(string watermark, Interaction<Unit, DirectoryInfo> dialogInteraction)
    {
        Watermark = watermark;
        
        OpenDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Directory = await dialogInteraction.Handle(Unit.Default);
            Content = Directory.FullName;
        });
    }
}