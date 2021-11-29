using System.Reactive;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2x_UI.Core;

public class Command : ReactiveObject
{
    public Command()
    {
        SetupCommandPreview();
    }
    
    private void SetupCommandPreview()
    {
        var livePreview = this.WhenAnyValue(
            x => x.InputImagePath,
            x => x.OutputImagePath,
            x => x.Suffix,
            x => x.Verbose,
            x => x.TTA,
            x => x.ScaleFactor,
            x => x.Denoise,
            x => x.OutputFileType,
            (x, y, z, q, t, a, b, c) => GetArguments());

        var observer = Observer.Create<string>(
            x => Preview = x,
            y => Preview = "Error occured when creating command preview",
            () => throw new InvalidOperationException());

        livePreview.Subscribe(observer);
    }
    
    public string GetArguments()
    {
        var command = new StringBuilder();

        command.Append("waifu-2x-ncnn-vulkan");
        
        if (string.IsNullOrEmpty(InputImagePath) || string.IsNullOrEmpty(OutputImagePath))
        {
            return command.ToString();
        }

        command.Append($" -input-path {InputImagePath}");
        
        command.Append($" -output-path {GetOutput()}");
        command.Append($" -format {OutputFileType.ToExtension()}");

        if (Denoise is not 0) command.Append($" -noise-level {Denoise}");

        if (ScaleFactor is not 2) command.Append($" -scale {ScaleFactor}");

        if (Verbose) command.Append(" -verbose");

        if (TTA) command.Append(" -x");

        return command.ToString();
    }

    private string GetOutput() => $"{OutputImagePath}{Suffix ?? string.Empty}.{OutputFileType.ToExtension()}";

    [Reactive] public string Preview { get; private set; } = string.Empty;
    
    // Image quality
    [Reactive] public int ScaleFactor { get; set; } = 2;
    [Reactive] public int Denoise { get; set; }
    
    // Output
    [Reactive] public string? Suffix { get; set; } = "-upscaled";
    [Reactive] public OutputFileType OutputFileType { get; set; } = OutputFileType.Png;
    
    // Other flags
    [Reactive] public bool Verbose { get; set; } = true;
    [Reactive] public bool TTA { get; set; }
    
    // Paths
    [Reactive] public string? InputImagePath { get; set; }
    [Reactive] public string? OutputImagePath { get; set; }

}