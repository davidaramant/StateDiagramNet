using System;
using System.Linq;
using NUnit.Framework;
using Sprache;
using StateDiagramCodeGen.ParsingModel;

namespace StateDiagramCodeGen.Tests
{
    [TestFixture]
    public class PlantUmlParserTests
    {
        [TestCase("->")]
        [TestCase("-->")]
        [TestCase("-u->")]
        [TestCase("-l->")]
        [TestCase("-r->")]
        [TestCase("-d->")]
        [TestCase("-up->")]
        [TestCase("-left->")]
        [TestCase("-right->")]
        [TestCase("-down->")]
        public void ShouldParseArrows(string arrow)
        {
            Assert.That(PlantUmlParser.Arrow.Parse(arrow), Is.EqualTo("->"));
        }

        [TestCase("MethodName", "MethodName")]
        [TestCase("MethodName()", "MethodName")]
        public void ShouldParseMethodReference(string input, string expected)
        {
            Assert.That(PlantUmlParser.FriendlyMethodReference.Parse(input), Is.EqualTo(expected));
        }

        [TestCase("Method Name", "MethodName")]
        [TestCase("A really long method name", "AReallyLongMethodName")]
        public void ShouldDehumanizeMethodSentence(string input, string expected)
        {
            Assert.That(PlantUmlParser.FriendlyMethodReference.Parse(input), Is.EqualTo(expected));
        }

        [TestCase("Alpha: SomeEvent / Action", "Alpha", "SomeEvent", "", "Action")]
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

            Assert.That(stateDescription.Source, Is.EqualTo(state));
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
            var entryAction = PlantUmlParser.InternalTransition.Parse(input);

            Assert.That(entryAction.Source, Is.EqualTo(state));
            Assert.That(entryAction.EventName, Is.EqualTo("entry"));
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
            var exitAction = PlantUmlParser.InternalTransition.Parse(input);

            Assert.That(exitAction.Source, Is.EqualTo(state));
            Assert.That(exitAction.EventName, Is.EqualTo("exit"));
            Assert.That(exitAction.GuardName, Is.EqualTo(guard));
            Assert.That(exitAction.ActionName, Is.EqualTo(action));
        }

        [TestCase("[*]-->Alpha", "Alpha", "")]
        [TestCase("[*]-->Alpha : /", "Alpha", "")]
        [TestCase("[*] --> Alpha : / Beta", "Alpha", "Beta")]
        [TestCase("[*] --> Alpha : /Beta", "Alpha", "Beta")]
        public void ShouldParseInitialTransition(string input, string destination, string action)
        {
            var initialTransition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(initialTransition.Source, Is.EqualTo("[*]"));
            Assert.That(initialTransition.Destination, Is.EqualTo(destination));
            Assert.That(initialTransition.EventName, Is.EqualTo(string.Empty));
            Assert.That(initialTransition.GuardName, Is.EqualTo(string.Empty));
            Assert.That(initialTransition.ActionName, Is.EqualTo(action));
        }

        [TestCase("Alpha-->[*]", "", "", "")]
        [TestCase("Alpha-->[*] : /", "", "", "")]
        [TestCase("Alpha --> [*] : / Beta", "", "", "Beta")]
        [TestCase("Alpha --> [*] : EventName [Guard] / Beta", "EventName", "Guard", "Beta")]
        public void ShouldParseTransitionsToFinal(string input, string eventName, string guard, string action)
        {
            var initialTransition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(initialTransition.Source, Is.EqualTo("Alpha"));
            Assert.That(initialTransition.Destination, Is.EqualTo("[*]"));
            Assert.That(initialTransition.EventName, Is.EqualTo(eventName));
            Assert.That(initialTransition.GuardName, Is.EqualTo(guard));
            Assert.That(initialTransition.ActionName, Is.EqualTo(action));
        }

        #region Event Transitions

        [TestCase("Alpha --> Beta : Gamma")]
        [TestCase("Alpha   -->    Beta   :   Gamma")]
        [TestCase("Alpha\t-->\tBeta\t:\tGamma")]
        [TestCase("Alpha-->Beta:Gamma")]
        public void ShouldParseSimpleEventTransition(string input)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.Empty);
            Assert.That(transition.ActionName, Is.Empty);
        }

        [TestCase("Alpha --> Beta : Gamma [Delta]", "Delta")]
        [TestCase("Alpha-->Beta:Gamma[Delta]", "Delta")]
        [TestCase("Alpha --> Beta : Gamma [  Condition Text ]", "ConditionText")]
        public void ShouldParseGuardedEventTransition(string input, string guard)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.EqualTo(guard));
            Assert.That(transition.ActionName, Is.Empty);
        }

        [TestCase("Alpha --> Beta : Gamma / Zeta", "Zeta")]
        [TestCase("Alpha-->Beta:Gamma/Zeta", "Zeta")]
        [TestCase("Alpha --> Beta : Gamma /  Action Text", "ActionText")]
        public void ShouldParseEventTransitionWithAction(string input, string action)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.Empty);
            Assert.That(transition.ActionName, Is.EqualTo(action));
        }

        [TestCase("Alpha --> Beta : Gamma [Delta] / Zeta", "Delta", "Zeta")]
        [TestCase("Alpha-->Beta:Gamma[Delta]/Zeta", "Delta", "Zeta")]
        [TestCase("Alpha --> Beta : Gamma [ Guard Text ] / Action Text", "GuardText", "ActionText")]
        public void ShouldParseGuardedEventTransitionWithAction(string input, string guard, string action)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            Assert.That(transition.Source, Is.EqualTo("Alpha"));
            Assert.That(transition.Destination, Is.EqualTo("Beta"));
            Assert.That(transition.EventName, Is.EqualTo("Gamma"));
            Assert.That(transition.GuardName, Is.EqualTo(guard));
            Assert.That(transition.ActionName, Is.EqualTo(action));
        }

        #endregion Event Transitions

        #region States

        [TestCase("state Alpha", "")]
        [TestCase("state Alpha", "")]
        [TestCase("state     Alpha", "")]
        [TestCase("state \"Longer Name\" as Alpha", "Longer Name")]
        public void ShouldParseSimpleStateDeclaration(string input, string longName)
        {
            var vertex = PlantUmlParser.State.End().Parse(input);

            Assert.That(vertex.ShortName, Is.EqualTo("Alpha"));
            Assert.That(vertex.LongName, Is.EqualTo(longName));
        }

        [TestCase("Alpha: SomeEvent / Action", typeof(InternalTransition))]
        [TestCase("[*]-->Alpha", typeof(ExternalTransition))]
        [TestCase("Alpha --> Beta : Gamma", typeof(ExternalTransition))]
        [TestCase("state Alpha", typeof(State))]
        public void ShouldParseStateComponent(string input, Type expectedType)
        {
            var element = PlantUmlParser.DiagramElement.End().Parse(input);

            Assert.That(element, Is.TypeOf(expectedType));
        }

        [Test]
        public void ShouldParseStateChildren()
        {
            var input = "{\n" +
                        "Alpha: entry / EntryAction\n" +
                        "Alpha: exit / ExitAction\n" +
                        "}";

            var contents = PlantUmlParser.StateChildren.Parse(input).ToList();
            
            Assert.That(contents, Has.Count.EqualTo(2));
            Assert.That(contents, Has.All.TypeOf<InternalTransition>());
        }

        [Test]
        public void ShouldParseStateWithEntryAndExitActions()
        {
            var input = "state Alpha {\n" +
                        "   Alpha: entry / EntryAction\n" +
                        "   Alpha: exit / ExitAction\n" +
                        "}";

            var state = PlantUmlParser.State.Parse(input);

            var contents = state.Contents.ToList();

            Assert.That(contents, Has.Count.EqualTo(2));
            Assert.That(contents, Has.All.TypeOf<InternalTransition>());
        }

        [Test]
        public void ShouldParseNestedSimpleState()
        {
            var input = "state Alpha {\n" +
                        "   state Beta\n" +
                        "}";

            var alphaState = PlantUmlParser.State.Parse(input);

            var alphaContents = alphaState.Contents.ToList();

            Assert.That(alphaContents, Has.Count.EqualTo(1), "Incorrect children for Alpha");
            Assert.That(alphaContents, Has.One.TypeOf<State>());
        }

        [Test]
        public void ShouldParseNestedCompositeState()
        {
            var input = "state Alpha {\n" +
                        "   state Beta {\n" +
                        "       state Gamma" +
                        "   }\n" +
                        "}";

            var alphaState = PlantUmlParser.State.End().Parse(input);

            var alphaContents = alphaState.Contents.ToList();

            Assert.That(alphaContents, Has.Count.EqualTo(1), "Incorrect children for Alpha");
            Assert.That(alphaContents, Has.One.TypeOf<State>());

            var betaState = (State)alphaContents.First();
            var betaContents = betaState.Contents.ToList();

            Assert.That(betaContents, Has.Count.EqualTo(1), "Incorrect children for Beta");
            Assert.That(betaContents, Has.One.TypeOf<State>());
        }

        #endregion States

        [Test]
        public void ShouldParseDiagram()
        {
            const string input = @"@startuml ""Simple Diagram""
                state Off
                state On {
                    state Idle
                    [*]->Idle
                    state Responding
                    Responding : entry / StartProcessing
                    Responding : exit / StopProcessing
                }
                On : entry / EnableLed
                On : exit / DisableLed
                [*] -> Off
                Off -> On : PowerToggle
                On --> Off : PowerToggle
                Idle --> Responding : ButtonPressed
                Responding --> Idle : DoneProcessing
                @enduml";

            var diagram = PlantUmlParser.Diagram.End().Parse(input);

            Assert.That(diagram.Name, Is.EqualTo("Simple Diagram"));
        }

        // TODO: single-line comments
        // TODO: multi-line comments
        // TODO: "hide empty description"
    }
}
