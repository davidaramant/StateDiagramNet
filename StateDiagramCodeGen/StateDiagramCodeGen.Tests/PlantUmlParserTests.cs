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
    }
}
