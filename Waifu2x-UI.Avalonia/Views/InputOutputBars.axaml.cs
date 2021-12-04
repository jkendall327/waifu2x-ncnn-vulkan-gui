using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2xUI.Avalonia.Views;

public class InputOutputBars : UserControl
{
    public InputOutputBars()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}