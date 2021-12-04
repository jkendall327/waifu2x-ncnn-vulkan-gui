using FluentAssertions;
using Waifu2x_UI.Core;
using Xunit;

namespace Waifu2x_UI.Tests;

public class CommandTests
{
    private readonly Command _sut = new();

    private readonly string _waifuPortion = "waifu-2x-ncnn-vulkan";
    
    [Fact]
    public void Preview_WhenCommandHasDefaultValues_ShouldJustBeWaifu2xName()
    {
        var actual = _sut.Preview;

        actual.Should().Be(_waifuPortion);
    }
}