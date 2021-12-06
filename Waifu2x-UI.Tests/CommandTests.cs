using System.Collections.Generic;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Linq;
using FluentAssertions;
using Waifu2x_UI.Core;
using Waifu2x_UI.Core.Commands;
using Xunit;

namespace Waifu2x_UI.Tests;

public class CommandTests
{
    private readonly Command _sut = new();

    private readonly string _waifuPortion = "waifu-2x-ncnn-vulkan";

    private readonly MockFileSystem _fileSystem;

    private readonly IDirectoryInfo _input;
    private readonly IDirectoryInfo _output;

    private readonly string _testFilepath = "/input/myfile.png";
    public CommandTests()
    {
        _fileSystem = new();

        _input = _fileSystem.Directory.CreateDirectory("input");
        _output = _fileSystem.Directory.CreateDirectory("output");
    }

    private List<IFileInfo> GetFiles() => _input.EnumerateFiles().ToList();

    private void SetInputAndOutput()
    {
        _fileSystem.File.Create(_testFilepath);

        _sut.InputImages = GetFiles();

        _sut.OutputDirectory = _output;
    }

    [Fact]
    public void Preview_WhenCommandHasDefaultValues_ShouldJustBeWaifu2xName()
    {
        var actual = _sut.Preview;

        actual.Should().Be(_waifuPortion);
    }


    [Fact]
    public void Preview_WhenGivenInputFile_ShouldBeDefaultOutput()
    {
        // TODO: the output path part here is wrong because the output directory
        // is only set to the default in the dialog handler, not in the actual command class.

        _fileSystem.File.Create(_testFilepath);

        _sut.InputImages = GetFiles();

        _sut.Preview.Should()
            .Be("waifu-2x-ncnn-vulkan -i \"/input/$IMAGE\" -output-path -o \"/$IMAGE-upscaled.png\" -format png -v");
    }

    [Fact]
    public void Preview_WhenGivenOutputButNotInput_ReturnsJustWaifuPortion()
    {
        _sut.OutputDirectory = _output;

        _sut.Preview.Should().Be(_waifuPortion);
    }

    [Fact]
    public void Preview_WhenGivenInputAndOutput_ShouldReflectBoth()
    {
        SetInputAndOutput();

        var expected = "waifu-2x-ncnn-vulkan -i \"/input/$IMAGE\" -output-path -o \"/output/$IMAGE-upscaled.png\" -format png -v";

        _sut.Preview.Should().Be(expected);
    }

    [Fact]
    public void Preview_WhenModelIsNotDefault_DisplaysModelName()
    {
        SetInputAndOutput();

        _sut.Model = "arbitraryName";

        var expected = "waifu-2x-ncnn-vulkan -i \"/input/$IMAGE\" -output-path -o \"/output/$IMAGE-upscaled.png\" -format png -v -m arbitraryName";

        _sut.Preview.Should().Be(expected);
    }

    [Fact]
    public void Preview_WhenGivenAllFlags_DisplaysCorrectly()
    {
        SetInputAndOutput();

        _sut.Model = "arbitraryName";
        _sut.Denoise = 2;
        _sut.Verbose = false;
        _sut.ScaleFactor = 4;
        _sut.TTA = true;
        _sut.Suffix = "__testSuffix__";

        var expected = "waifu-2x-ncnn-vulkan -i \"/input/$IMAGE\" -output-path -o \"/output/$IMAGE__testSuffix__.png\" -format png -n 2 -s 4 -x -m arbitraryName";

        _sut.Preview.Should().Be(expected);
    }
}