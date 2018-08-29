using NUnit.Framework;
using Sprache;
using StateDiagramCodeGen.Model;

namespace StateDiagramCodeGen.Tests
{
    [TestFixture]
    public class PlantUmlParserTests
    {
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

        [TestCase("state Alpha")]
        [TestCase("   state Alpha")]
        [TestCase("state     Alpha")]
        public void ShouldParseSimpleStateDeclaration(string input)
        {
            var vertex = PlantUmlParser.SimpleStateDeclaration.Parse(input);

            Assert.That(vertex.Name, Is.EqualTo("Alpha"));
        }

        [TestCase("State : Event", "State", "Event", "", "")]
        [TestCase("State : Event /", "State", "Event", "", "")]
        [TestCase("State : Event / Action", "State", "Event", "", "Action")]
        [TestCase("State : Event [Guard] /", "State", "Event", "Guard", "")]
        [TestCase("State : Event [Guard]", "State", "Event", "Guard", "")]
        [TestCase("State : Event [Guard] / Action", "State", "Event", "Guard", "Action")]
        [TestCase("State:Event[Guard]/Action", "State", "Event", "Guard", "Action")]
        public void ShouldParseInternalTransition(string input, string state, string eventName, string guard, string action)
        {
            var stateDescription = PlantUmlParser.InternalTransition.Parse(input);

            Assert.That(stateDescription.StateName, Is.EqualTo(state));
            Assert.That(stateDescription.EventName, Is.EqualTo(eventName));
            Assert.That(stateDescription.GuardName, Is.EqualTo(guard));
            Assert.That(stateDescription.ActionName, Is.EqualTo(action));
        }

        [TestCase("State : entry", "State", "", "")]
        [TestCase("State : entry /", "State", "", "")]
        [TestCase("State : entry/", "State", "", "")]
        [TestCase("State : entry / Action", "State", "", "Action")]
        [TestCase("State : entry [Guard] /", "State", "Guard", "")]
        [TestCase("State : entry [Guard]", "State", "Guard", "")]
        [TestCase("State : entry [Guard] / Action", "State", "Guard", "Action")]
        [TestCase("State:entry[Guard]/Action", "State", "Guard", "Action")]
        public void ShouldParseEntryAction(string input, string state, string guard, string action)
        {
            var entryAction = PlantUmlParser.EntryAction.Parse(input);

            Assert.That(entryAction.StateName, Is.EqualTo(state));
            Assert.That(entryAction.GuardName, Is.EqualTo(guard));
            Assert.That(entryAction.ActionName, Is.EqualTo(action));
        }

        [TestCase("State : exit", "State", "", "")]
        [TestCase("State : exit /", "State", "", "")]
        [TestCase("State : exit/", "State", "", "")]
        [TestCase("State : exit / Action", "State", "", "Action")]
        [TestCase("State : exit [Guard] /", "State", "Guard", "")]
        [TestCase("State : exit [Guard]", "State", "Guard", "")]
        [TestCase("State : exit [Guard] / Action", "State", "Guard", "Action")]
        [TestCase("State:exit[Guard]/Action", "State", "Guard", "Action")]
        public void ShouldParseExitAction(string input, string state, string guard, string action)
        {
            var exitAction = PlantUmlParser.ExitAction.Parse(input);

            Assert.That(exitAction.StateName, Is.EqualTo(state));
            Assert.That(exitAction.GuardName, Is.EqualTo(guard));
            Assert.That(exitAction.ActionName, Is.EqualTo(action));
        }

        #region Event Transitions

        [TestCase("    Alpha --> Beta : Gamma")]
        [TestCase("    Alpha   -->    Beta   :   Gamma")]
        [TestCase("\tAlpha\t-->\tBeta\t:\tGamma")]
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
        [TestCase("    Alpha --> Beta : Gamma [  Condition Text ]", "ConditionText")]
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
        [TestCase("Alpha --> Beta : Gamma /  Action Text", "ActionText")]
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

        #endregion Event Transitions
    }
}
