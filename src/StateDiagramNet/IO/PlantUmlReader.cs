using Sprache;

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
        from star in Parse.String(Constants.Star)
        from _ in SpacesOrTabs
        select Constants.Star;

    internal static readonly Parser<string> Identifier =
        Parse.Identifier(Parse.Letter, Parse.LetterOrDigit).AndTrailingPadding();

    private static readonly Parser<string> QuotedString =
        Parse.LetterOrDigit.Or(Parse.Char(' ')).Many().Contained(Parse.Char('"'), Parse.Char('"')).Text();

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

    internal static readonly Parser<IOption<string>> StartDiagram =
        from start in Parse.String("@startuml").AndTrailingWhitespace()
        from name in QuotedString.Optional()
        from trailing in Parse.WhiteSpace.Many()
        select name;

    private static readonly Parser<string> EndDiagram =
        from end in Parse.String("@enduml").AndTrailingWhitespace()
        select "end";
}
