using FluentAssertions;
using Sprache;
using StateDiagramNet.IO;
using StateDiagramNet.PlantUmlModel;
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
    [InlineData("Alpha:Description")]
    [InlineData("Alpha : Description")]
    public void ShouldParseStateDescription(string text)
    {
        var description = PlantUmlReader.StateDescription.Parse(text);
        description.StateName.Should().Be("Alpha");
        description.Description.Should().Be("Description");
    }

    [Theory]
    [InlineData("Alpha -> Beta")]
    [InlineData("Alpha: Description")]
    [InlineData("state Alpha")]
    public void ShouldParseDiagramElement(string text)
    {
        var _ = PlantUmlReader.DiagramElement.Parse(text);
    }
    
    [Theory]
    [InlineData("state Alpha")]
    [InlineData("state Alpha {}")]
    public void ShouldParseEmptyStateDefinition(string text)
    {
        var state = (StateDefinition)PlantUmlReader.StateDefinition.Parse(text);
        state.Name.Should().Be("Alpha");
    }

    [Fact]
    public void ShouldParseStateChildren()
    {
        var children = PlantUmlReader.StateChildren.Parse(
            "{\n" +
            "Alpha: Something\n" +
            "[*] -> Beta\n" +
            "state Beta\n" +
            "}");

        children.Should().HaveCount(3);
    }

    [Fact]
    public void ShouldParseStateWithChildren()
    {
        var state = (StateDefinition)PlantUmlReader.StateDefinition.Parse(
            "state Alpha {\n" +
            "\tAlpha: Something\n" +
            "\tstate Beta\n" +
            "\t[*] -> Beta\n" +
            "}");

        state.Children.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("@startuml \"Simple Diagram\"", "Simple Diagram")]
    [InlineData("@startuml", null)]
    public void ShouldParseStartDiagram(string text, string? expectedName)
        => PlantUmlReader.StartDiagram.Parse(text).GetOrDefault().Should().Be(expectedName);
}
