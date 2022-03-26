using FluentAssertions;
using Sprache;
using StateDiagramNet.IO;
using Xunit;

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
    [InlineData("Alpha -> Beta", "Alpha", "Beta")]
    [InlineData("Alpha->Beta", "Alpha", "Beta")]
    [InlineData("[*] -> Beta", "[*]", "Beta")]
    [InlineData("Alpha -> [*]", "Alpha", "[*]")]
    public void ShouldParseTransition(string text, string source, string destination)
    {
        var transition = PlantUmlReader.Transition.Parse(text);

        transition.Source.Should().Be(source);
        transition.Destination.Should().Be(destination);
        transition.Description.Should().BeNull();
    }

    [Theory]
    [InlineData("Alpha -> Beta : Description", "Description")]
    [InlineData("Alpha -> Beta:Description", "Description")]
    [InlineData("Alpha -> Beta : eventName / [Guard] Action", "eventName / [Guard] Action")]
    public void ShouldParseTransitionWithDescription(string text, string description)
    {
        var transition = PlantUmlReader.Transition.Parse(text);

        transition.Source.Should().Be("Alpha");
        transition.Destination.Should().Be("Beta");
        transition.Description.Should().Be(description);
    }

    [Theory]
    [InlineData("@startuml \"Simple Diagram\"", "Simple Diagram")]
    [InlineData("@startuml", null)]
    public void ShouldParseStartDiagram(string text, string? expectedName)
        => PlantUmlReader.StartDiagram.Parse(text).GetOrDefault().Should().Be(expectedName);
}
