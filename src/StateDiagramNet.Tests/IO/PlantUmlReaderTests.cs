using Xunit;
using FluentAssertions;
using StateDiagramNet.IO;
using Sprache;

namespace StateDiagramNet.Tests.IO;
public sealed class PlantUmlReaderTests
{
    [Theory]
    [InlineData("->")]
    [InlineData("-->")]
    [InlineData("-u->")]
    [InlineData("-l->")]
    [InlineData("-r->")]
    [InlineData("-d->")]
    [InlineData("-up->")]
    [InlineData("-left->")]
    [InlineData("-right->")]
    [InlineData("-down->")]
    public void ShouldParseArrows(string arrow) 
        => PlantUmlReader.Arrow.Parse(arrow).Should().Be("->");

    [Theory]
    [InlineData("@startuml \"Simple Diagram\"", "Simple Diagram")]
    public void ShouldParseStartDiagram(string text, string expectedName) 
        => PlantUmlReader.StartDiagram.Parse(text).Should().Be(expectedName);
}
