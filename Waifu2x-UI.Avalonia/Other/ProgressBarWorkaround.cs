using System;
using Avalonia;
using Avalonia.Controls;

namespace Waifu2xUI.Avalonia.Other;

/// <summary>
/// https://github.com/AvaloniaUI/Avalonia/issues/2881
/// </summary>
public class ProgressBarWorkaround
{
    public static AvaloniaProperty<double> ValueProperty =
        AvaloniaProperty.RegisterAttached<ProgressBarWorkaround, ProgressBar, double>("Value");

    public static void SetValue(ProgressBar pb, double value) =>
        pb.SetValue(ValueProperty, value);
    static ProgressBarWorkaround()
    {
        ValueProperty.Changed.Subscribe(ev =>
        {
            ((ProgressBar) ev.Sender).Value = ev.NewValue.Value;
        });
    }
}