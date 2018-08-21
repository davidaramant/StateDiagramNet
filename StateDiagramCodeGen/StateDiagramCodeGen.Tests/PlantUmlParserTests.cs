using NUnit.Framework;
using Sprache;
using StateDiagramCodeGen.Model;

namespace StateDiagramCodeGen.Tests
{
    [TestFixture]
    public class PlantUmlParserTests
    {
        [Test]
        public void ShouldParseSimpleStateDeclaration()
        {
            var vertex = PlantUmlParser.SimpleStateDeclaration.Parse("state Alpha");

            Assert.That(vertex.Name, Is.EqualTo("Alpha"));
        }

        [TestCase("MethodName", "MethodName")]
        [TestCase("MethodName()", "MethodName")]
        public void ShouldParseMethodReference(string input, string expected)
        {
            Assert.That(PlantUmlParser.MethodReference.Parse(input), Is.EqualTo(expected));
        }

        [TestCase("Method Name", "MethodName")]
        [TestCase("A really long method name", "AReallyLongMethodName")]
        public void ShouldDehumanizeMethodSentence(string input, string expected)
        {
            Assert.That(PlantUmlParser.DehumanizedSentence.Parse(input), Is.EqualTo(expected));
        }

        [TestCase("    Alpha --> Beta : Gamma")]
        [TestCase("Alpha-->Beta:Gamma")]
        public void ShouldParseSimpleEventTransition(string input)
        {
            var transition = PlantUmlParser.EventTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.Empty);
            Assert.That(transition.ActionName, Is.Empty);
        }

        [TestCase("    Alpha --> Beta : Gamma [Delta]", "Delta")]
        [TestCase("Alpha-->Beta:Gamma[Delta]", "Delta")]
        [TestCase("    Alpha --> Beta : Gamma [ Some Condition Text ]", "SomeConditionText")]
        public void ShouldParseGuardedEventTransition(string input, string guard)
        {
            var transition = PlantUmlParser.EventTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.EqualTo(guard));
            Assert.That(transition.ActionName, Is.Empty);
        }

        [TestCase("    Alpha --> Beta : Gamma / Zeta", "Zeta")]
        [TestCase("Alpha-->Beta:Gamma/Zeta", "Zeta")]
        [TestCase("Alpha --> Beta : Gamma / Some Action Text", "SomeActionText")]
        public void ShouldParseEventTransitionWithAction(string input, string action)
        {
            var transition = PlantUmlParser.EventTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.Empty);
            Assert.That(transition.ActionName, Is.EqualTo(action));
        }

        [TestCase("    Alpha --> Beta : Gamma [Delta] / Zeta", "Delta", "Zeta")]
        [TestCase("Alpha-->Beta:Gamma[Delta]/Zeta", "Delta", "Zeta")]
        [TestCase("Alpha --> Beta : Gamma [ Guard Text ] / Action Text", "GuardText", "ActionText")]
        public void ShouldParseGuardedEventTransitionWithAction(string input, string guard, string action)
        {
            var transition = PlantUmlParser.EventTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.EqualTo(guard));
            Assert.That(transition.ActionName, Is.EqualTo(action));
        }
    }
}
