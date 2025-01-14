using System.Collections.Generic;
using System.Linq;
using static Pidgin.Parser;
using static Pidgin.Parser<char, Pidgin.Unit>;

namespace Pidgin.Examples.Xml
{
    public static class XmlParser
    {
        public static Result<char, Unit, Tag> Parse(string input) => Node.Parse(input);

        static readonly Parser<char, Unit, string> Identifier =
            from first in Token(char.IsLetter)
            from rest in Token(char.IsLetterOrDigit).ManyString()
            select first + rest;

        static readonly Parser<char, Unit, char> LT = Char('<');
        static readonly Parser<char, Unit, char> GT = Char('>');
        static readonly Parser<char, Unit, char> Quote = Char('"');
        static readonly Parser<char, Unit, char> Equal = Char('=');
        static readonly Parser<char, Unit, char> Slash = Char('/');
        static readonly Parser<char, Unit, Unit> SlashGT =
            Slash.Then(Whitespaces).Then(GT).Then(Return(Unit.Value));
        static readonly Parser<char, Unit, Unit> LTSlash = 
            LT.Then(Whitespaces).Then(Slash).Then(Return(Unit.Value));
        
        static readonly Parser<char, Unit, string> AttrValue = 
            Token(c => c != '"').ManyString();

        static readonly Parser<char, Unit, Attribute> Attr = 
            from name in Identifier
            from eq in Equal.Between(SkipWhitespaces)
            from val in AttrValue.Between(Quote)
            select new Attribute(name, val);

        static readonly Parser<char, Unit, OpeningTagInfo> TagBody =
            from name in Identifier
            from attrs in (
                from ws in Try(Whitespace.SkipAtLeastOnce())
                from attrs in Attr.Separated(SkipWhitespaces)
                select attrs
            ).Optional()
            select new OpeningTagInfo(name, attrs.GetValueOrDefault(Enumerable.Empty<Attribute>()));

        static readonly Parser<char, Unit, Tag> EmptyElementTag =
            from opening in LT
            from body in TagBody.Between(SkipWhitespaces)
            from closing in SlashGT
            select new Tag(body.Name, body.Attributes, null);

        static readonly Parser<char, Unit, OpeningTagInfo> OpeningTag =
            TagBody
                .Between(SkipWhitespaces)
                .Between(LT, GT);

        static Parser<char, Unit, string> ClosingTag =>
            Identifier
                .Between(SkipWhitespaces)
                .Between(LTSlash, GT);

        static readonly Parser<char, Unit, Tag> Tag =
            from open in OpeningTag
            from children in Try(Node!).Separated(SkipWhitespaces).Between(SkipWhitespaces)
            from close in ClosingTag
            where open.Name.Equals(close)
            select new Tag(open.Name, open.Attributes, children);
    
        static readonly Parser<char, Unit, Tag> Node = Try(EmptyElementTag).Or(Tag);

        private struct OpeningTagInfo
        {
            public string Name { get; }
            public IEnumerable<Attribute> Attributes { get; }

            public OpeningTagInfo(string name, IEnumerable<Attribute> attributes)
            {
                Name = name;
                Attributes = attributes;
            }
        }
    }
}