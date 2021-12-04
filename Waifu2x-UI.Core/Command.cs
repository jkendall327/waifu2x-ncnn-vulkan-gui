using System.Reactive;
using System.Text;
using System.Text.Json.Serialization;
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
            x => x.InputImages,
            x => x.OutputDirectory,
            x => x.Suffix,
            x => x.Verbose,
            x => x.TTA,
            x => x.ScaleFactor,
            x => x.Denoise,
            x => x.OutputFileType,
            x => x.Model,
            (_, _, _, _, _, _, _, _, _) => GenerateArguments());

        var observer = Observer.Create<string>(
            x => Preview = x,
            _ => Preview = "Error occured when creating command preview",
            () => throw new InvalidOperationException());

        livePreview.Subscribe(observer);
    }

    public string GenerateArguments()
    {
        var command = new StringBuilder("waifu-2x-ncnn-vulkan");

        if (!InputImages.Any()) return command.ToString();

        var inputDir = InputImages.First().Directory?.FullName;
        
        command.Append($" -i \"{inputDir}{Path.DirectorySeparatorChar}$IMAGE\"");

        command.Append(" -output-path");
        
        AppendOutputPath(command);
        
        command.Append($" -format {OutputFileType.ToExtension()}");
        
        AppendFlags(command);

        return command.ToString();
    }

    private void AppendOutputPath(StringBuilder command)
    {
        command.Append(" -o ");

        command.Append('"');
        
        command.Append(OutputDirectory?.FullName);

        command.Append(Path.DirectorySeparatorChar);
        
        command.Append("$IMAGE");

        if (!string.IsNullOrEmpty(Suffix)) command.Append(Suffix);

        command.Append('.');
        
        command.Append(OutputFileType.ToExtension());

        command.Append('"');
    }
    
    private void AppendFlags(StringBuilder command)
    {
        if (Denoise is not 0) command.Append($" -n {Denoise}");

        if (ScaleFactor is not 2) command.Append($" -s {ScaleFactor}");

        if (Verbose) command.Append(" -v");

        if (TTA) command.Append(" -x");

        if (Model is not "models-cunet") command.Append($" -m {Model}");
    }

    [Reactive] public string Preview { get; private set; } = string.Empty;
    
    // Image quality
    [Reactive] public int ScaleFactor { get; set; } = 2;
    [Reactive] public int Denoise { get; set; }
    
    // Output
    [Reactive] public string? Suffix { get; set; } = "-upscaled";
    [Reactive] public OutputFileType OutputFileType { get; set; } = OutputFileType.Png;
    
    // Model
    [Reactive] public string Model { get; set; } = "models-cunet"; 
    
    // Other flags
    [Reactive] public bool Verbose { get; set; } = true;
    [Reactive] [JsonIgnore] public bool TTA { get; set; }
    
    // Paths
    [Reactive] [JsonIgnore] public List<FileInfo> InputImages { get; set; } = new();
    [Reactive] [JsonIgnore] public DirectoryInfo? OutputDirectory { get; set; }
}