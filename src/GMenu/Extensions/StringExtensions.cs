namespace GMenu.Extensions;

public static class StringExtensions
{
    public static bool ContainsOnlyLatinAndNumbersAndUnderline(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return true;

        return value.All(c => c is >= 'a' and <= 'z' or >= 'A' and <= 'Z' or >= '0' and <= '9' or '!' or '@' or '#'
            or '$' or '%' or '^' or '&' or '*' or '(' or ')' or '-' or '_' or '+' or '=' or '[' or ']' or '{' or '}'
            or ';' or ':' or '\'' or '"' or ',' or '.' or '<' or '>' or '/' or '?' or '|' or '`' or '~' or ' ');
    }
}