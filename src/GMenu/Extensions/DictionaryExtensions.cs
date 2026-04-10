using System.Collections.Generic;

namespace GMenu.Extensions;

public static class DictionaryExtensions
{
    public static Dictionary<TKey, TValue> AddFluent<TKey, TValue>(this Dictionary<TKey, TValue> dictionary, TKey key, TValue value) where TKey : notnull
    {
        dictionary.Add(key, value);
        return dictionary;
    }
}