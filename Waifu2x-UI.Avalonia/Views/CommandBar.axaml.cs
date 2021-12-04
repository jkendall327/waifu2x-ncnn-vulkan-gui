using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2xUI.Avalonia.Views;

public class CommandBar : UserControl
{
    public CommandBar()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}