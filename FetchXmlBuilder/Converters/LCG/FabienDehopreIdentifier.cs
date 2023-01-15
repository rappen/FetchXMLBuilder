// Stolen with love from 
// https://gist.github.com/FabienDehopre/5245476

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public static class IdentifierExtensions
{
    // definition of a valid C# identifier: http://msdn.microsoft.com/en-us/library/aa664670(v=vs.71).aspx
    private const string FORMATTING_CHARACTER = @"\p{Cf}";
    private const string CONNECTING_CHARACTER = @"\p{Pc}";
    private const string DECIMAL_DIGIT_CHARACTER = @"\p{Nd}";
    private const string COMBINING_CHARACTER = @"\p{Mn}|\p{Mc}";
    private const string LETTER_CHARACTER = @"\p{Lu}|\p{Ll}|\p{Lt}|\p{Lm}|\p{Lo}|\p{Nl}";

    private const string IDENTIFIER_PART_CHARACTER = LETTER_CHARACTER + "|" +
                                                     DECIMAL_DIGIT_CHARACTER + "|" +
                                                     CONNECTING_CHARACTER + "|" +
                                                     COMBINING_CHARACTER + "|" +
                                                     FORMATTING_CHARACTER;

    private const string IDENTIFIER_PART_CHARACTERS = "(" + IDENTIFIER_PART_CHARACTER + ")+";
    private const string IDENTIFIER_START_CHARACTER = "(" + LETTER_CHARACTER + "|_)";

    private const string IDENTIFIER_OR_KEYWORD = IDENTIFIER_START_CHARACTER + "(" +
                                                 IDENTIFIER_PART_CHARACTERS + ")*";

    // C# keywords: http://msdn.microsoft.com/en-us/library/x53a06bb(v=vs.71).aspx
    private static readonly HashSet<string> _keywords = new HashSet<string>
    {
        "abstract",  "event",      "new",        "struct",
        "as",        "explicit",   "null",       "switch",
        "base",      "extern",     "object",     "this",
        "bool",      "false",      "operator",   "throw",
        "break",     "finally",    "out",        "true",
        "byte",      "fixed",      "override",   "try",
        "case",      "float",      "params",     "typeof",
        "catch",     "for",        "private",    "uint",
        "char",      "foreach",    "protected",  "ulong",
        "checked",   "goto",       "public",     "unchecked",
        "class",     "if",         "readonly",   "unsafe",
        "const",     "implicit",   "ref",        "ushort",
        "continue",  "in",         "return",     "using",
        "decimal",   "int",        "sbyte",      "virtual",
        "default",   "interface",  "sealed",     "volatile",
        "delegate",  "internal",   "short",      "void",
        "do",        "is",         "sizeof",     "while",
        "double",    "lock",       "stackalloc",
        "else",      "long",       "static",
        "enum",      "namespace",  "string"
    };

    private static readonly Regex _validIdentifierRegex = new Regex("^" + IDENTIFIER_OR_KEYWORD + "$", RegexOptions.Compiled);

    public static bool IsValidIdentifier(this string identifier)
    {
        if (String.IsNullOrWhiteSpace(identifier)) return false;
        
        var normalizedIdentifier = identifier.Normalize();

        // 1. check that the identifier match the validIdentifer regex and it's not a C# keyword
        if (_validIdentifierRegex.IsMatch(normalizedIdentifier) && !_keywords.Contains(normalizedIdentifier))
        {
            return true;
        }

        // 2. check if the identifier starts with @
        if (normalizedIdentifier.StartsWith("@") && _validIdentifierRegex.IsMatch(normalizedIdentifier.Substring(1)))
        {
            return true;
        }

        // 3. it's not a valid identifier
        return false;
    }
}