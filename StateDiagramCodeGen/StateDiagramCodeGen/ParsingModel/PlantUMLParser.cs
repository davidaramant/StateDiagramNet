using System.Collections.Generic;
using System.Linq;
using Humanizer;
using Sprache;

namespace StateDiagramCodeGen.ParsingModel
{
    public static class PlantUmlParser
    {
        private static readonly Parser<string> SpacesOrTabs = from ws in Parse.Chars(' ', '\t').Many() select " ";

        private static Parser<T> AndTrailingPadding<T>(this Parser<T> parser) =>
            from _ in parser
            from trailing in SpacesOrTabs
            select _;

        private static Parser<T> AndTrailingWhitespace<T>(this Parser<T> parser) =>
            from _ in parser
            from trailing in Parse.WhiteSpace.Many()
            select _;

        private static readonly Parser<string> Star = 
            from star in Parse.String("[*]")
            from _ in SpacesOrTabs
            select "[*]";

        public static readonly Parser<string> Identifier =
            Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).AndTrailingPadding();
        
        public static readonly Parser<string> DehumanizedSentence =
            from sentence in Parse.LetterOrDigit.Or(Parse.Char(' ')).Many().Text().Token()
            from _ in SpacesOrTabs
            select sentence.Dehumanize();

        public static readonly Parser<string> MethodReference =
            from id in Parse.Identifier(Parse.Letter, Parse.LetterOrDigit)
            from parens in Parse.String("()")
            from _ in SpacesOrTabs
            select id;

        public static readonly Parser<string> FriendlyMethodReference = DehumanizedSentence.Or(MethodReference);

        public static readonly Parser<string> Arrow =
            from qualifier in
                Parse.String("-up-")
                .Or(Parse.String("-down-"))
                .Or(Parse.String("-left-"))
                .Or(Parse.String("-right-"))
                .Or(Parse.String("-u-"))
                .Or(Parse.String("-d-"))
                .Or(Parse.String("-l-"))
                .Or(Parse.String("-r-"))
                .Or(Parse.String("--"))
                .Or(Parse.String("-"))
            from end in Parse.String(">")
            from _ in SpacesOrTabs
            select "->";

        private static Parser<char> CharWithTrailingPadding(char c) => Parse.Char(c).AndTrailingPadding();
        private static Parser<char> CharWithTrailingWhitespace(char c) => Parse.Char(c).AndTrailingWhitespace();
        
        private static readonly Parser<string> Guard =
            from openGuard in CharWithTrailingPadding('[')
            from guard in FriendlyMethodReference
            from closeGuard in CharWithTrailingPadding(']')
            select guard;

        private static readonly Parser<string> Action =
            from openAction in CharWithTrailingPadding('/')
            from action in FriendlyMethodReference
            select action;

        private static readonly Parser<ExternalTransition> DecoratedTransition =
            from source in Identifier
            from arrow in Arrow
            from destination in Star.Or(Identifier)
            from colon in CharWithTrailingPadding(':')
            from eventName in Identifier.Optional()
            from guardFunction in Guard.Optional()
            from actionFunction in Action.Optional()
            select new ExternalTransition(
                source: source,
                destination: destination,
                eventName: eventName.GetOrElse(string.Empty),
                guardName: guardFunction.GetOrElse(string.Empty),
                actionName: actionFunction.GetOrElse(string.Empty));

        private static readonly Parser<ExternalTransition> UndecoratedTransition =
            from source in Identifier
            from arrow in Arrow
            from destination in Star.Or(Identifier)
            select new ExternalTransition(
                source: source,
                destination: destination,
                eventName: string.Empty,
                guardName: string.Empty,
                actionName: string.Empty);

        private static readonly Parser<ExternalTransition> InitialTransitionWithNoAction =
            from source in Star
            from arrow in Arrow
            from destination in Identifier
            select new ExternalTransition(
                source: source,
                destination: destination,
                eventName: string.Empty,
                guardName: string.Empty,
                actionName: string.Empty);

        private static readonly Parser<ExternalTransition> InitialTransitionWithAction =
            from source in Star
            from arrow in Arrow
            from destination in Identifier
            from colon in CharWithTrailingPadding(':')
            from actionFunction in Action
            select new ExternalTransition(
                source: source,
                destination: destination,
                eventName: string.Empty,
                guardName: string.Empty,
                actionName: actionFunction);

        private static readonly Parser<ExternalTransition> InitialTransition =
            InitialTransitionWithAction.Or(InitialTransitionWithNoAction);

        public static readonly Parser<ExternalTransition> ExternalTransition =
            InitialTransition.Or(DecoratedTransition).Or(UndecoratedTransition);

        public static readonly Parser<InternalTransition> InternalTransition =
            from leading in SpacesOrTabs
            from stateName in Identifier
            from colon in CharWithTrailingPadding(':')
            from eventName in Identifier
            from guardFunction in Guard.Optional()
            from actionFunction in Action.Optional()
            select new InternalTransition(
                source: stateName,
                eventName: eventName,
                guardName: guardFunction.GetOrElse(string.Empty),
                actionName: actionFunction.GetOrElse(string.Empty));

        public static readonly Parser<string> QuotedString =
            Parse.LetterOrDigit.Or(Parse.Char(' ')).Many().Contained(Parse.Char('"'), Parse.Char('"')).Text();

        public static readonly Parser<string> LongStateName =
            from longName in QuotedString.AndTrailingPadding()
            from asKeyword in Parse.String("as").AndTrailingPadding()
            select longName;
        
        public static readonly Parser<StateDefinition> State =
            from state in Parse.String("state").AndTrailingPadding()
            from longName in LongStateName.Optional()
            from shortName in Identifier
            from children in StateChildren.Optional()
            select new StateDefinition(
                shortName: shortName,
                longName: longName.GetOrElse(string.Empty),
                contents: children.GetOrElse(Enumerable.Empty<IDiagramElement>()));

        private static readonly Parser<IDiagramElement> StateDiagramElement =
            from state in State
            select (IDiagramElement)state;

        public static readonly Parser<IDiagramElement> DiagramElement =
            from element in StateDiagramElement
                .Or(InternalTransition)
                .Or(ExternalTransition)
            from trailing in Parse.WhiteSpace.Many()
            select element;

        public static readonly Parser<IEnumerable<IDiagramElement>> StateChildren =
            from openParen in CharWithTrailingWhitespace('{')
            from children in DiagramElement.Many()
            from closeParen in CharWithTrailingWhitespace('}')
            select children;

        static readonly CommentParser Comment = new CommentParser("'", "/'", "'/");

        private static readonly Parser<IOption<string>> StartDiagram =
            from start in Parse.String("@startuml").AndTrailingWhitespace()
            from name in QuotedString.Optional()
            from trailing in Parse.WhiteSpace.Many()
            select name;

        static readonly Parser<string> EndDiagram = 
            from end in Parse.String("@enduml").AndTrailingWhitespace()
            select "end";

        private static readonly Parser<string> HideEmptyDescriptions =
            Parse.String("hide empty description").Token().Text();

        public static readonly Parser<Diagram> Diagram =
            from diagramName in StartDiagram
            from hideEmptyDescription in HideEmptyDescriptions.Optional()
            from elements in DiagramElement.Many()
            from end in EndDiagram
            select new Diagram(diagramName.GetOrElse("Unnamed"), elements);
    }
}
