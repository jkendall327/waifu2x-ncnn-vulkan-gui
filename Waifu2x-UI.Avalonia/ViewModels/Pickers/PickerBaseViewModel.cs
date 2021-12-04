using ReactiveUI.Fody.Helpers;

namespace Waifu2xUI.Avalonia.ViewModels;

public class PickerBaseViewModel : ViewModelBase
{
    protected string Watermark { get; }    
    [Reactive] public string Content { get; protected set; } = string.Empty;

    protected PickerBaseViewModel(string watermark)
    {
        Watermark = watermark;
    }
}