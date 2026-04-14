using GMenu.Extensions;

namespace GMenu.Testing;

public class StringExtensionsTests
{
    [Theory]
    [InlineData("testValue", "val", true)]
    [InlineData("testValue", "valE", false)]
    [InlineData("testValue", "VAL", true)]
    [InlineData("testValue", "testval", true)]
    public void ContainsRange_ReturnsExpectedResult(string value, string searchPattern,  bool expectedResult)
    {
        //Arrange
        var span = searchPattern.AsSpan();
        //Act
        var result = value.ContainsRange(span);
        //Assert
        Assert.Equal(expectedResult, result);
    }
}