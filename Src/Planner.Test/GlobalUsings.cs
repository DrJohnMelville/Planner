global using TUnit;
global using FluentAssertions;
global using Moq;

namespace TUnit;

public static class Assert
{
    public static void Equal<T>(T expected, T actual) => actual.Should().Be(expected);
    public static void NotEqual<T>(T expected, T actual) => actual.Should().NotBe(expected);

    public static void Empty<T>(IEnumerable<T> item) => item.Should().BeEmpty();
    public static void Single<T>(IEnumerable<T> item) => item.Should().HaveCount(1);
    public static void Contains(string expected, string actual) => actual.Should().Contain(expected);
    public static void DoesNotContain(string expected, string actual) => actual.Should().NotContain(expected);
    public static void Null(object? o) => o.Should().BeNull();
    public static void Same(object expected, object actual) => actual.Should().BeSameAs(expected);
    public static void NotSame(object expected, object actual) => actual.Should().NotBeSameAs(expected);
    public static void False(bool i) => i.Should().BeFalse();
}