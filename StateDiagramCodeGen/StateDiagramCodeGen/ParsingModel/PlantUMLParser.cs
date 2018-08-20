using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sprache;

namespace StateDiagramCodeGen.ParsingModel
{
    public static class PlantUMLParser
    {
        static CommentParser Comment = new CommentParser("'", "/'", "'/");

        public static Parser<string> StartDiagram = Parse.String("@startuml").Token().Text();
        public static Parser<string> EndDiagram = Parse.String("@enduml").Token().Text();

        public static Parser<string> StateKeyword = Parse.String("state").Token().Text();

        public static Parser<string> Identifier =
            from leading in Parse.WhiteSpace.Many()
            from first in Parse.Letter.Once()
            from rest in Parse.LetterOrDigit.Many()
            from trailing in Parse.WhiteSpace.Many()
            select new string(first.Concat(rest).ToArray());

        public static Parser<Vertex> SimpleStateDeclaration =
            from state in StateKeyword
            from stateName in Identifier
            select new Vertex(stateName);
    }
}
