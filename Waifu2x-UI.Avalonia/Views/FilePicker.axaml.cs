using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2x_UI.Avalonia.Views;

public class FilePicker : UserControl
{
    public FilePicker()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}