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
        OutputDirectory = DirectoryExtensions.GetOutputDirectory();
        
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
            (_, _, _, _, _, _, _, _) => GetPreview());

        var observer = Observer.Create<string>(
            x => Preview = x,
            _ => Preview = "Error occured when creating command preview",
            () => throw new InvalidOperationException());

        livePreview.Subscribe(observer);
    }

    public string GenerateArguments(FileInfo file)
    {
        var command = new StringBuilder();

        command.Append("waifu-2x-ncnn-vulkan");
        
        command.Append($" -i {file.FullName}");

        AppendOutputPath(command, file);
        
        AppendFlags(command);

        return command.ToString();
    }

    private string GetPreview()
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

        command.Append($" -output-path {GetPreviewOutputPath()}");
        command.Append($" -format {OutputFileType.ToExtension()}");

        AppendFlags(command);

        return command.ToString();
    }

    private void AppendFlags(StringBuilder command)
    {
        if (Denoise is not 0) command.Append($" -n {Denoise}");

        if (ScaleFactor is not 2) command.Append($" -s {ScaleFactor}");

        if (Verbose) command.Append(" -v");

        if (TTA) command.Append(" -x");
    }

    private void AppendOutputPath(StringBuilder command, FileInfo file)
    {
        command.Append($" -o ");
        
        command.Append(OutputDirectory.FullName);

        command.Append(Path.DirectorySeparatorChar);

        var name = Path.GetFileNameWithoutExtension(file.Name);
        
        command.Append(name);

        if (!string.IsNullOrEmpty(Suffix)) command.Append(Suffix);

        command.Append('.');
        
        command.Append(OutputFileType.ToExtension());
        
        command.Append($" -format {OutputFileType.ToExtension()}");
    }

    private string GetPreviewOutputPath()
    {
        var sb = new StringBuilder();
        
        sb.Append(OutputDirectory.FullName);

        sb.Append(Path.DirectorySeparatorChar);
        
        sb.Append(InputImages.Count is 1 ? InputImages.First().Name : "[image]");

        if (!string.IsNullOrEmpty(Suffix)) sb.Append(Suffix);

        sb.Append('.');
        
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
    [Reactive] [JsonIgnore] public bool TTA { get; set; }
    
    // Paths
    [Reactive] [JsonIgnore] public List<FileInfo> InputImages { get; set; } = new();
    [Reactive] [JsonIgnore] public DirectoryInfo OutputDirectory { get; set; }
}