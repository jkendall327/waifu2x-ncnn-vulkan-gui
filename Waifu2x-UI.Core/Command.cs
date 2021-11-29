using System.IO.Abstractions;
using System.Reactive;
using System.Text;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace Waifu2x_UI.Core;

public class Command : ReactiveObject
{
    public Command()
    {
        OutputDirectory = SetDefaultOutputDirectory();
        
        SetupCommandPreview();
    }

    private DirectoryInfo SetDefaultOutputDirectory()
    {
        var fullPath = Path.GetFullPath(Directory.GetCurrentDirectory());
        var separator = Path.DirectorySeparatorChar;

        var path = $"{fullPath}{separator}output";
        Directory.CreateDirectory(path);

        return new(path);
    }

    private void SetupCommandPreview()
    {
        var livePreview = this.WhenAnyValue(
            x => x.InputImages,
            x => x.OutputDirectory,
            x => x.Suffix,
            x => x.Verbose,
            x => x.TTA,
            x => x.ScaleFactor,
            x => x.Denoise,
            x => x.OutputFileType,
            (x, y, z, q, t, a, b, c) => GetPreview());

        var observer = Observer.Create<string>(
            x => Preview = x,
            y => Preview = "Error occured when creating command preview",
            () => throw new InvalidOperationException());

        livePreview.Subscribe(observer);
    }
    
    public string GetPreview()
    {
        var command = new StringBuilder();

        command.Append("waifu-2x-ncnn-vulkan");
        
        if (!InputImages.Any())
        {
            return command.ToString();
        }

        if (InputImages.Count is 1)
        {
            command.Append($" -input-path {InputImages.First().FullName}");
        }
        else
        {
            command.Append(" -input-path [image]");
        }
        
        command.Append($" -output-path {GetOutputPath()}");
        command.Append($" -format {OutputFileType.ToExtension()}");

        if (Denoise is not 0) command.Append($" -noise-level {Denoise}");

        if (ScaleFactor is not 2) command.Append($" -scale {ScaleFactor}");

        if (Verbose) command.Append(" -verbose");

        if (TTA) command.Append(" -x");

        return command.ToString();
    }

    private string GetOutputPath()
    {
        var sb = new StringBuilder();
        
        sb.Append(OutputDirectory.FullName);

        sb.Append(InputImages.Count is 1 ? InputImages.First().Name : "[image]");

        if (!string.IsNullOrEmpty(Suffix)) sb.Append(Suffix);

        sb.Append(OutputFileType.ToExtension());
        
        return sb.ToString();
    }

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
    [Reactive] public List<FileInfo> InputImages { get; set; } = new();
    [Reactive] public DirectoryInfo OutputDirectory { get; set; }
}