using FluentAssertions;
using Sprache;
using StateDiagramNet.ParsingModel;
using System;
using System.Linq;
using Xunit;

namespace StateDiagramNet.Tests
{
    public sealed class PlantUmlParserTests
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
        {
            PlantUmlParser.Arrow.Parse(arrow).Should().Be("->");
        }

        [Theory]
        [InlineData("MethodName", "MethodName")]
        [InlineData("MethodName()", "MethodName")]
        public void ShouldParseMethodReference(string input, string expected)
        {
            PlantUmlParser.FriendlyMethodReference.Parse(input).Should().Be(expected);
        }

        [Theory]
        [InlineData("Method Name", "MethodName")]
        [InlineData("A really long method name", "AReallyLongMethodName")]
        public void ShouldDehumanizeMethodSentence(string input, string expected)
        {
            PlantUmlParser.FriendlyMethodReference.Parse(input).Should().Be(expected);
        }

        [Theory]
        [InlineData("Alpha: SomeEvent / Action", "Alpha", "SomeEvent", "", "Action")]
        [InlineData("State : Event", "State", "Event", "", "")]
        [InlineData("State : Event /", "State", "Event", "", "")]
        [InlineData("State : Event / Action", "State", "Event", "", "Action")]
        [InlineData("State : Event [Guard] /", "State", "Event", "Guard", "")]
        [InlineData("State : Event [Guard]", "State", "Event", "Guard", "")]
        [InlineData("State : Event [Guard] / Action", "State", "Event", "Guard", "Action")]
        [InlineData("State:Event[Guard]/Action", "State", "Event", "Guard", "Action")]
        public void ShouldParseInternalTransition(string input, string state, string eventName, string guard, string action)
        {
            var stateDescription = PlantUmlParser.InternalTransition.Parse(input);

            stateDescription.Source.Should().Be(state);
            stateDescription.EventName.Should().Be(eventName);
            stateDescription.GuardName.Should().Be(guard);
            stateDescription.ActionName.Should().Be(action);
        }

        [Theory]
        [InlineData("State : entry", "State", "", "")]
        [InlineData("State : entry /", "State", "", "")]
        [InlineData("State : entry/", "State", "", "")]
        [InlineData("State : entry / Action", "State", "", "Action")]
        [InlineData("State : entry [Guard] /", "State", "Guard", "")]
        [InlineData("State : entry [Guard]", "State", "Guard", "")]
        [InlineData("State : entry [Guard] / Action", "State", "Guard", "Action")]
        [InlineData("State:entry[Guard]/Action", "State", "Guard", "Action")]
        public void ShouldParseEntryAction(string input, string state, string guard, string action)
        {
            var entryAction = PlantUmlParser.InternalTransition.Parse(input);

            entryAction.Source.Should().Be(state);
            entryAction.EventName.Should().Be("entry");
            entryAction.GuardName.Should().Be(guard);
            entryAction.ActionName.Should().Be(action);
        }

        [Theory]
        [InlineData("State : exit", "State", "", "")]
        [InlineData("State : exit /", "State", "", "")]
        [InlineData("State : exit/", "State", "", "")]
        [InlineData("State : exit / Action", "State", "", "Action")]
        [InlineData("State : exit [Guard] /", "State", "Guard", "")]
        [InlineData("State : exit [Guard]", "State", "Guard", "")]
        [InlineData("State : exit [Guard] / Action", "State", "Guard", "Action")]
        [InlineData("State:exit[Guard]/Action", "State", "Guard", "Action")]
        public void ShouldParseExitAction(string input, string state, string guard, string action)
        {
            var exitAction = PlantUmlParser.InternalTransition.Parse(input);

            exitAction.Source.Should().Be(state);
            exitAction.EventName.Should().Be("exit");
            exitAction.GuardName.Should().Be(guard);
            exitAction.ActionName.Should().Be(action);
        }

        [Theory]
        [InlineData("[*]-->Alpha", "Alpha", "")]
        [InlineData("[*]-->Alpha : /", "Alpha", "")]
        [InlineData("[*] --> Alpha : / Beta", "Alpha", "Beta")]
        [InlineData("[*] --> Alpha : /Beta", "Alpha", "Beta")]
        public void ShouldParseInitialTransition(string input, string destination, string action)
        {
            var initialTransition = PlantUmlParser.ExternalTransition.Parse(input);

            initialTransition.Source.Should().Be("[*]");
            initialTransition.Destination.Should().Be(destination);
            initialTransition.EventName.Should().Be(string.Empty);
            initialTransition.GuardName.Should().Be(string.Empty);
            initialTransition.ActionName.Should().Be(action);
        }

        [Theory]
        [InlineData("Alpha-->[*]", "", "", "")]
        [InlineData("Alpha-->[*] : /", "", "", "")]
        [InlineData("Alpha --> [*] : / Beta", "", "", "Beta")]
        [InlineData("Alpha --> [*] : EventName [Guard] / Beta", "EventName", "Guard", "Beta")]
        public void ShouldParseTransitionsToFinal(string input, string eventName, string guard, string action)
        {
            var initialTransition = PlantUmlParser.ExternalTransition.Parse(input);

            initialTransition.Source.Should().Be("Alpha");
            initialTransition.Destination.Should().Be("[*]");
            initialTransition.EventName.Should().Be(eventName);
            initialTransition.GuardName.Should().Be(guard);
            initialTransition.ActionName.Should().Be(action);
        }

        #region Event Transitions

        [Theory]
        [InlineData("Alpha --> Beta : Gamma")]
        [InlineData("Alpha   -->    Beta   :   Gamma")]
        [InlineData("Alpha\t-->\tBeta\t:\tGamma")]
        [InlineData("Alpha-->Beta:Gamma")]
        public void ShouldParseSimpleEventTransition(string input)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            transition.Source.Should().Be("Alpha");
            transition.Destination.Should().Be("Beta");
            transition.EventName.Should().Be("Gamma");
            transition.GuardName.Should().BeEmpty();
            transition.ActionName.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Alpha --> Beta : Gamma [Delta]", "Delta")]
        [InlineData("Alpha-->Beta:Gamma[Delta]", "Delta")]
        [InlineData("Alpha --> Beta : Gamma [  Condition Text ]", "ConditionText")]
        public void ShouldParseGuardedEventTransition(string input, string guard)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            transition.Source.Should().Be("Alpha");
            transition.Destination.Should().Be("Beta");
            transition.EventName.Should().Be("Gamma");
            transition.GuardName.Should().Be(guard);
            transition.ActionName.Should().BeEmpty();
        }

        [Theory]
        [InlineData("Alpha --> Beta : Gamma / Zeta", "Zeta")]
        [InlineData("Alpha-->Beta:Gamma/Zeta", "Zeta")]
        [InlineData("Alpha --> Beta : Gamma /  Action Text", "ActionText")]
        public void ShouldParseEventTransitionWithAction(string input, string action)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            transition.Source.Should().Be("Alpha");
            transition.Destination.Should().Be("Beta");
            transition.EventName.Should().Be("Gamma");
            transition.GuardName.Should().BeEmpty();
            transition.ActionName.Should().Be(action);
        }

        [Theory]
        [InlineData("Alpha --> Beta : Gamma [Delta] / Zeta", "Delta", "Zeta")]
        [InlineData("Alpha-->Beta:Gamma[Delta]/Zeta", "Delta", "Zeta")]
        [InlineData("Alpha --> Beta : Gamma [ Guard Text ] / Action Text", "GuardText", "ActionText")]
        public void ShouldParseGuardedEventTransitionWithAction(string input, string guard, string action)
        {
            var transition = PlantUmlParser.ExternalTransition.Parse(input);

            transition.Source.Should().Be("Alpha");
            transition.Destination.Should().Be("Beta");
            transition.EventName.Should().Be("Gamma");
            transition.GuardName.Should().Be(guard);
            transition.ActionName.Should().Be(action);
        }

        #endregion Event Transitions

        #region States

        [Theory]
        [InlineData("state Alpha", "")]
        [InlineData("state     Alpha", "")]
        [InlineData("state \"Longer Name\" as Alpha", "Longer Name")]
        public void ShouldParseSimpleStateDeclaration(string input, string longName)
        {
            var vertex = PlantUmlParser.State.End().Parse(input);

            vertex.ShortName.Should().Be("Alpha");
            vertex.LongName.Should().Be(longName);
        }

        [Theory]
        [InlineData("Alpha: SomeEvent / Action", typeof(InternalTransition))]
        [InlineData("[*]-->Alpha", typeof(ExternalTransition))]
        [InlineData("Alpha --> Beta : Gamma", typeof(ExternalTransition))]
        [InlineData("state Alpha", typeof(StateDefinition))]
        public void ShouldParseStateComponent(string input, Type expectedType)
        {
            var element = PlantUmlParser.DiagramElement.End().Parse(input);

            element.Should().BeOfType(expectedType);
        }

        [Fact]
        public void ShouldParseStateChildren()
        {
            var input = "{\n" +
                        "Alpha: entry / EntryAction\n" +
                        "Alpha: exit / ExitAction\n" +
                        "}";

            var contents = PlantUmlParser.StateChildren.Parse(input).ToList();

            contents.Should().HaveCount(2).And.AllBeOfType<InternalTransition>();
        }

        [Fact]
        public void ShouldParseStateWithEntryAndExitActions()
        {
            var input = "state Alpha {\n" +
                        "   Alpha: entry / EntryAction\n" +
                        "   Alpha: exit / ExitAction\n" +
                        "}";

            var state = PlantUmlParser.State.Parse(input);

            var contents = state.Contents.ToList();

            contents.Should().HaveCount(2).And.AllBeOfType<InternalTransition>();
        }

        [Fact]
        public void ShouldParseNestedSimpleState()
        {
            var input = "state Alpha {\n" +
                        "   state Beta\n" +
                        "}";

            var alphaState = PlantUmlParser.State.Parse(input);

            var alphaContents = alphaState.Contents.ToList();

            alphaContents.Should().HaveCount(1).And.AllBeOfType<StateDefinition>();
        }

        [Fact]
        public void ShouldParseNestedCompositeState()
        {
            var input = "state Alpha {\n" +
                        "   state Beta {\n" +
                        "       state Gamma" +
                        "   }\n" +
                        "}";

            var alphaState = PlantUmlParser.State.End().Parse(input);

            var alphaContents = alphaState.Contents.ToList();

            alphaContents.Should().HaveCount(1).And.AllBeOfType<StateDefinition>();

            var betaState = (StateDefinition)alphaContents.First();
            var betaContents = betaState.Contents.ToList();

            betaContents.Should().HaveCount(1).And.AllBeOfType<StateDefinition>();
        }

        #endregion States

        [Fact]
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

            diagram.Name.Should().Be("Simple Diagram");
        }

        [Fact]
        public void ShouldParseDiagramWithHideEmptyDescription()
        {
            const string input = @"@startuml ""Simple Diagram""
                hide empty description
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

            diagram.Name.Should().Be("Simple Diagram");
        }

        // TODO: single-line comments
        // TODO: multi-line comments
        // TODO: "hide empty description"
    }
}
