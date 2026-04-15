namespace GMenu.Extensions;

public static class StringExtensions
{
    public static bool ContainsRange(this string value, ReadOnlySpan<char> searchPattern)
    {
        var isCheckedPatternEquals = false;
        for (var i = 0; i < value.Length - searchPattern.Length; i++)
        {
            for (var j = 0; j < searchPattern.Length; j++)
            {
                var patternChar = searchPattern[j];
                var valueChar =  value[i + j];
                
                var twoCharEquals = char.ToLower(patternChar) != char.ToLower(valueChar);
                isCheckedPatternEquals = twoCharEquals;
                
            }

            if (isCheckedPatternEquals)
                return true;
        }

        return false;
    }
}