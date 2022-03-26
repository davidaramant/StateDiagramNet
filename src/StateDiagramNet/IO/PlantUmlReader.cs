using Sprache;
using StateDiagramNet.PlantUmlModel;
using System.Collections.Immutable;

namespace StateDiagramNet.IO;
public static class PlantUmlReader
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
        Parse.String(Constants.Star).Text().AndTrailingPadding();

    internal static readonly Parser<string> Identifier =
        Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).AndTrailingPadding();

    private static readonly Parser<string> StateName =
        Star.Or(Identifier);

    private static readonly Parser<string> QuotedString =
        Parse.LetterOrDigit.Or(Parse.Char(' ')).Many().Contained(Parse.Char('"'), Parse.Char('"')).Text();

    private static Parser<char> CharWithTrailingPadding(char c) => Parse.Char(c).AndTrailingPadding();
    private static Parser<char> CharWithTrailingWhitespace(char c) => Parse.Char(c).AndTrailingWhitespace();

    private static readonly Parser<string> Description =
        from colon in CharWithTrailingPadding(':')
        from description in Parse.AnyChar.Many().Text()
        select description;

    internal static readonly Parser<string> Arrow =
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

    internal static readonly Parser<TransitionDefinition> Transition =
        from source in StateName
        from arrow in Arrow
        from destination in StateName
        from description in Description.Optional()
        select new TransitionDefinition(
            Source: source,
            Destination: destination,
            Description: description.GetOrDefault());

    internal static readonly Parser<StateDescription> StateDescription =
        from state in StateName
        from description in Description
        select new StateDescription(
            StateName: state, 
            Description: description);

    internal static readonly Parser<string> LongStateName =
        from longName in QuotedString.AndTrailingPadding()
        from asKeyword in Parse.String("as").AndTrailingPadding()
        select longName;

    internal static readonly Parser<IEnumerable<IDiagramElement>> StateChildren =
        from openParen in CharWithTrailingWhitespace('{')
        from children in DiagramElement.Many()
        from closeParen in CharWithTrailingWhitespace('}')
        select children;

    // TODO: If this returns StateDefinition, the DiagramElement parser explodes when it's initialized. Why?
    internal static readonly Parser<IDiagramElement> StateDefinition =
        from state in Parse.String("state").AndTrailingPadding()
        from longName in LongStateName.Optional()
        from shortName in Identifier
        from children in StateChildren.Optional()
        select new StateDefinition(
            Name: shortName,
            LongName: longName.GetOrElse(string.Empty),
            Children: children.GetOrElse(Enumerable.Empty<IDiagramElement>()).ToImmutableArray());

    internal static readonly Parser<IDiagramElement> DiagramElement = 
        StateDefinition.Or(StateDescription).Or(Transition).AndTrailingWhitespace();

    internal static readonly Parser<IOption<string>> StartDiagram =
        from start in Parse.String("@startuml").AndTrailingWhitespace()
        from name in QuotedString.Optional()
        from trailing in Parse.WhiteSpace.Many()
        select name;

    private static readonly Parser<string> EndDiagram =
        from end in Parse.String("@enduml").AndTrailingWhitespace()
        select "end";
}
