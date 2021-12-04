using System.IO;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2xUI.Avalonia.ViewModels;

public class DirectoryPickerViewModel : PickerBaseViewModel
{
    [Reactive] public DirectoryInfo? Directory { get; private set; }
    private ReactiveCommand<Unit, Unit> OpenDialogCommand { get; }

    public DirectoryPickerViewModel(string watermark, Interaction<Unit, DirectoryInfo> dialogInteraction) : base(watermark)
    {
        OpenDialogCommand = ReactiveCommand.CreateFromTask(async () =>
        {
            Directory = await dialogInteraction.Handle(Unit.Default);
            Content = Directory.FullName;
        });
    }
}