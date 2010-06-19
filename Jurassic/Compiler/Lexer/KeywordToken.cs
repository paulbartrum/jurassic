using System;
using System.Collections.Generic;

namespace Jurassic.Compiler
{

    /// <summary>
    /// Represents a reserved word in the source code.
    /// </summary>
    internal class KeywordToken : Token
    {
        /// <summary>
        /// Creates a new KeywordToken instance.
        /// </summary>
        /// <param name="name"> The keyword name. </param>
        public KeywordToken(string name)
        {
            if (name == null)
                throw new ArgumentNullException("name");
            this.Name = name;
        }

        /// <summary>
        /// Gets the name of the identifier.
        /// </summary>
        public string Name
        {
            get;
            private set;
        }

        // Keywords.
        public readonly static KeywordToken Break = new KeywordToken("break");
        public readonly static KeywordToken Case = new KeywordToken("case");
        public readonly static KeywordToken Catch = new KeywordToken("catch");
        public readonly static KeywordToken Continue = new KeywordToken("continue");
        public readonly static KeywordToken Debugger = new KeywordToken("debugger");
        public readonly static KeywordToken Default = new KeywordToken("default");
        public readonly static KeywordToken Delete = new KeywordToken("delete");
        public readonly static KeywordToken Do = new KeywordToken("do");
        public readonly static KeywordToken Else = new KeywordToken("else");
        public readonly static KeywordToken Finally = new KeywordToken("finally");
        public readonly static KeywordToken For = new KeywordToken("for");
        public readonly static KeywordToken Function = new KeywordToken("function");
        public readonly static KeywordToken If = new KeywordToken("if");
        public readonly static KeywordToken In = new KeywordToken("in");
        public readonly static KeywordToken InstanceOf = new KeywordToken("instanceof");
        public readonly static KeywordToken New = new KeywordToken("new");
        public readonly static KeywordToken Return = new KeywordToken("return");
        public readonly static KeywordToken Switch = new KeywordToken("switch");
        public readonly static KeywordToken This = new KeywordToken("this");
        public readonly static KeywordToken Throw = new KeywordToken("throw");
        public readonly static KeywordToken Try = new KeywordToken("try");
        public readonly static KeywordToken Typeof = new KeywordToken("typeof");
        public readonly static KeywordToken Var = new KeywordToken("var");
        public readonly static KeywordToken Void = new KeywordToken("void");
        public readonly static KeywordToken While = new KeywordToken("while");
        public readonly static KeywordToken With = new KeywordToken("with");

        // Reserved words.
        public readonly static KeywordToken Class = new KeywordToken("class");
        public readonly static KeywordToken Const = new KeywordToken("const");
        public readonly static KeywordToken Enum = new KeywordToken("enum");
        public readonly static KeywordToken Export = new KeywordToken("export");
        public readonly static KeywordToken Extends = new KeywordToken("extends");
        public readonly static KeywordToken Import = new KeywordToken("import");
        public readonly static KeywordToken Super = new KeywordToken("super");

        // Strict reserved words.
        public readonly static KeywordToken Implements = new KeywordToken("implements");
        public readonly static KeywordToken Interface = new KeywordToken("interface");
        public readonly static KeywordToken Let = new KeywordToken("let");
        public readonly static KeywordToken Package = new KeywordToken("package");
        public readonly static KeywordToken Private = new KeywordToken("private");
        public readonly static KeywordToken Protected = new KeywordToken("protected");
        public readonly static KeywordToken Public = new KeywordToken("public");
        public readonly static KeywordToken Static = new KeywordToken("static");
        public readonly static KeywordToken Yield = new KeywordToken("yield");


        // Mapping from text -> keyword.
        private readonly static Dictionary<string, Token> lookupTable = new Dictionary<string, Token>()
        {
            { "break",		Break },
            { "case",		Case },
            { "catch",		Catch },
            { "continue",	Continue },
            { "debugger",	Debugger },
            { "default",	Default },
            { "delete",		Delete },
            { "do",		    Do },
            { "else",		Else },
            { "finally",	Finally },
            { "for",		For },
            { "function",	Function },
            { "if",		    If },
            { "in",		    In },
            { "instanceof",	InstanceOf },
            { "new",		New },
            { "return",		Return },
            { "switch",		Switch },
            { "this",		This },
            { "throw",		Throw },
            { "try",		Try },
            { "typeof",		Typeof },
            { "var",		Var },
            { "void",		Void },
            { "while",		While },
            { "with",		With },

            // Reserved words.
            { "class",		Class },
            { "const",		Const },
            { "enum",		Enum },
            { "export",		Export },
            { "extends",	Extends },
            { "import",		Import },
            { "super",      Super },

            // Strict reserved words.
            { "implements",	Implements },
            { "interface",	Interface },
            { "let",		Let },
            { "package",	Package },
            { "private",	Private },
            { "protected",	Protected },
            { "public",		Public },
            { "static",		Static },
            { "yield",		Yield },

            // Literal keywords.
            { "true",		LiteralToken.True },
            { "false",		LiteralToken.False },
            { "null",		LiteralToken.Null },
        };

        /// <summary>
        /// Creates a token from the given string.
        /// </summary>
        /// <param name="text"> The text. </param>
        /// <returns> The token corresponding to the given string, or <c>null</c> if the string
        /// does not represent a valid token. </returns>
        public static Token FromString(string text)
        {
            Token result;
            if (lookupTable.TryGetValue(text, out result) == false)
                return new IdentifierToken(text);
            return result;
        }

        /// <summary>
        /// Gets a string that represents the token in a parseable form.
        /// </summary>
        public override string Text
        {
            get { return this.Name; }
        }
    }

}