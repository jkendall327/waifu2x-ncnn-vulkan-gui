using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Waifu2xUI.Avalonia.Views;

public class SmoothProgressBar : UserControl
{
    public SmoothProgressBar()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}