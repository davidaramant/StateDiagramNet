using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Sprache;

namespace StateDiagramCodeGen.ParsingModel
{
    public static class PlantUMLParser
    {
        static readonly CommentParser Comment = new CommentParser("'", "/'", "'/");

        public static readonly Parser<string> StartDiagram = Parse.String("@startuml").Token().Text();
        public static readonly Parser<string> EndDiagram = Parse.String("@enduml").Token().Text();

        public static readonly Parser<string> StateKeyword = Parse.String("state").Token().Text();

        public static readonly Parser<string> Identifier =
            from leading in Parse.WhiteSpace.Many()
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.Many()
            from trailing in Parse.WhiteSpace.Many()
            select new string(first.Concat(rest).ToArray());

        public static readonly Parser<Vertex> SimpleStateDeclaration =
            from state in StateKeyword
            from stateName in Identifier
            select new Vertex(stateName);

        private static readonly Parser<string> Arrow = Parse.String("-->").Token().Text();

        public static readonly Parser<EventTransition> EventTransition =
            from source in Identifier
            from arrow in Arrow
            from destination in Identifier
            from colon in Parse.Char(':')
            from eventName in Identifier
            select new EventTransition(source, destination, eventName);
    }
}
