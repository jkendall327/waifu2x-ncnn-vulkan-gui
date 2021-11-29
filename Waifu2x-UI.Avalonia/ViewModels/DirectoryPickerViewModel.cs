using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2xUI.Avalonia.ViewModels;

public class DirectoryPickerViewModel : ViewModelBase
{
    private string Watermark { get; }    
    [Reactive] private string Content { get; set; } = string.Empty;
    [Reactive] public DirectoryInfo? Directory { get; private set; }
    
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