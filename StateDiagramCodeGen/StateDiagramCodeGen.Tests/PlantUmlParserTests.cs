using NUnit.Framework;
using Sprache;
using StateDiagramCodeGen.ParsingModel;

namespace StateDiagramCodeGen.Tests
{
    [TestFixture]
    public class PlantUmlParserTests
    {
        [Test]
        public void ShouldParseSimpleStateDeclaration()
        {
            var vertex = PlantUMLParser.SimpleStateDeclaration.Parse("state Alpha");

            Assert.That(vertex.Name, Is.EqualTo("Alpha"));
        }

        [TestCase("    Alpha --> Beta : Gamma")]
        [TestCase("Alpha-->Beta:Gamma")]
        public void ShouldParseSimpleEventTransition(string input)
        {
            var transition = PlantUMLParser.EventTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.Empty);
            Assert.That(transition.ActionName, Is.Empty);
        }
    }
}
