using static Avalonia.Application;

namespace GMenu.Views.Utils;

public static class ResourceDictionaryUtil
{
    public static void Replace(string oldName, string newName)
    {
        FormatToSource(newName, out var newSource);
        FormatToSource(oldName, out var oldSource);
        
        var rootResources = Current!.Resources;
        if (rootResources is not ResourceDictionary rootDict)
            return;
        
        
        var mergedDictionaries = rootDict.MergedDictionaries;
        
        if(mergedDictionaries.Any(res => res is ResourceInclude include && include.Source == new Uri(newSource)))
               return;
        
        var indexToReplace = 0;
        for (var i = 0; i < mergedDictionaries.Count; i++)
        {
            if (mergedDictionaries[i] is not ResourceInclude include)
                continue;
            if (include.Source?.OriginalString != oldSource)
                continue;
            
            indexToReplace = i;
            break;
        }
        
        var newInclude = new ResourceInclude(new Uri(StaticConfiguration.AvaloniaResourcePrefix)) { Source = new Uri(newSource, UriKind.Relative) };
        
        mergedDictionaries[indexToReplace] = newInclude;
    }

    

    private static void FormatToSource(string name, out string result)
    {
        result = Path.Combine(StaticConfiguration.DefaultLocalizationPathPrefix, name + ".axaml");
    }
}