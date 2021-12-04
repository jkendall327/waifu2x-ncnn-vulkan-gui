using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2xUI.Avalonia.Views;

public class CommandTerminal : UserControl
{
    public CommandTerminal()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}