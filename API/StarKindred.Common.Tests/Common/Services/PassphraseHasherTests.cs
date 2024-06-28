using FluentAssertions;
using StarKindred.Common.Services;
using Xunit;

namespace StarKindred.Common.Tests.Common.Services;

public class PassphraseHasherTests
{
    [Theory]
    [InlineData("")]
    [InlineData("something a little bit longer")]
    [InlineData("emoji should also work: 💩")]
    [InlineData("Gs+2oHZmjKe8~4XA4MjVk")]
    public void HashAndVerifyAgree(string input)
    {
        var sut = new PassphraseHasher();
        
        var hash = sut.Hash(input);
        sut.Verify(input, hash).Should().BeTrue();
    }
}