using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2xUI.Avalonia.Views;

public class Title : UserControl
{
    public Title()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}