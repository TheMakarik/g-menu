namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileEntityReader(
    IConfigurationProvider configurationProvider,
    IDesktopFileReader reader,
    ILogger logger)
    : IDesktopFileEntityReader
{
    public async Task<DesktopFile?> ReadDesktopFileAsync(string path, CancellationToken cancellationToken = default)
    {
        logger.Information("Reading desktop file async: {path}", path);
        
        if (!File.Exists(path))
        {
            logger.Error("Desktop file not found: {path}", path);
            return null;
        }

        var configuration = configurationProvider.CurrentObservable;
        var lines = reader.ReadEntry(path);
        
        var desktopFile = new DesktopFile
        {
            Path = path,
            Name = Path.GetFileNameWithoutExtension(path)
        };

        var wasFoundLocalizedName = false;

        await foreach (var line in lines.ToAsyncEnumerable().WithCancellation(cancellationToken))
        {
            if (string.IsNullOrEmpty(line))
                continue;

            var equalsIndex = line.IndexOf('=');
            if (equalsIndex == -1)
                continue;

            var key = line.AsSpan(0, equalsIndex);
            var value = line.AsSpan(equalsIndex + 1);

            if (key[^1] == ']')
            {
                if (configuration.LocalizeDesktopFiles)
                {
                    if (IsLocalizedKey(key, DesktopFileKeys.NameKey, CultureInfo.CurrentCulture))
                    {
                        desktopFile.Name = value.ToString();
                        wasFoundLocalizedName = true;
                        desktopFile.NameKey = key.ToString();
                    }
                    else if (IsLocalizedKey(key, DesktopFileKeys.CommentKey, CultureInfo.CurrentCulture))
                    {
                        desktopFile.Comment = value.ToString();
                    }
                    else if (IsLocalizedKey(key, DesktopFileKeys.GenericNameKey, CultureInfo.CurrentCulture))
                    {
                        desktopFile.GenericName = value.ToString();
                    }
                    else if (IsLocalizedKey(key, DesktopFileKeys.KeywordsKey, CultureInfo.CurrentCulture))
                    {
                        desktopFile.Keywords = value.ToString();
                    }
                }
                continue;
            }

            switch (key)
            {
                case DesktopFileKeys.NameKey:
                    if (wasFoundLocalizedName)
                        desktopFile.UnlocalizedName = value.ToString();
                    else
                    {
                        desktopFile.Name = value.ToString().Replace("\\n", " ");
                        desktopFile.NameKey = DesktopFileKeys.NameKey;
                    }
                    break;
                case DesktopFileKeys.IconKey:
                    desktopFile.IconPath = value.ToString();
                    break;
                case DesktopFileKeys.CategoriesKey:
                    var categoriesValue = value.ToString();
                    desktopFile.Categories = categoriesValue;
                    var semicolonIndex = categoriesValue.IndexOf(';');
                    desktopFile.Category = semicolonIndex == -1
                        ? categoriesValue
                        : categoriesValue[..semicolonIndex];
                    break;
                case DesktopFileKeys.ExecKey:
                    desktopFile.Exec = value.ToString();
                    break;
                case DesktopFileKeys.NoDisplayKey:
                    if (bool.TryParse(value, out var noDisplay))
                    {
                        desktopFile.NoDisplay = noDisplay;
                    }
                    break;
                case DesktopFileKeys.HiddenKey:
                    if (bool.TryParse(value, out var hidden))
                        desktopFile.Hidden = hidden;
                    break;
                case DesktopFileKeys.TerminalKey:
                    if (bool.TryParse(value, out var terminal))
                        desktopFile.Terminal = terminal;
                    break;
                case DesktopFileKeys.CommentKey:
                    if (desktopFile.Comment is null)
                        desktopFile.Comment = value.ToString().Replace("\\n", " ");
                    break;
                case DesktopFileKeys.GenericNameKey:
                    if (desktopFile.GenericName is null)
                        desktopFile.GenericName = value.ToString().Replace("\\n", " ");
                    break;
                case DesktopFileKeys.KeywordsKey:
                    if (desktopFile.Keywords is null)
                        desktopFile.Keywords = value.ToString();
                    break;
                case DesktopFileKeys.VersionKey:
                    desktopFile.Version = value.ToString();
                    break;
                case DesktopFileKeys.DBusActivatableKey:
                    if (bool.TryParse(value, out var dbusActivatable))
                        desktopFile.DBusActivatable = dbusActivatable;
                    break;
                case DesktopFileKeys.StartupNotifyKey:
                    if (bool.TryParse(value, out var startupNotify))
                        desktopFile.StartupNotify = startupNotify;
                    break;
                case DesktopFileKeys.StartupWmClassKey:
                    desktopFile.StartupWMClass = value.ToString();
                    break;
                case DesktopFileKeys.MimeTypeKey:
                    desktopFile.MimeType = value.ToString();
                    break;
                case DesktopFileKeys.SingleMainWindowKey:
                    if (bool.TryParse(value, out var singleMainWindow))
                        desktopFile.SingleMainWindow = singleMainWindow;
                    break;
                case DesktopFileKeys.ImplementsKey:
                    desktopFile.Implements = value.ToString();
                    break;
                case DesktopFileKeys.TryExecKey:
                    desktopFile.TryExec = value.ToString();
                    break;
                case DesktopFileKeys.OnlyShowInKey:
                    desktopFile.OnlyShowIn = value.ToString();
                    break;
                case DesktopFileKeys.NotShowInKey:
                    desktopFile.NotShowIn = value.ToString();
                    break;
                case DesktopFileKeys.XGnomeAutoRestartKey:
                    if (bool.TryParse(value, out var gnomeAutoRestart))
                        desktopFile.XGnomeAutoRestart = gnomeAutoRestart;
                    break;
                case DesktopFileKeys.XGnomeUsesNotificationsKey:
                    desktopFile.XGnomeUsesNotifications = value.ToString();
                    break;
                case DesktopFileKeys.PathKey:
                    desktopFile.WorkingDirectory = value.ToString();
                    break;
                case DesktopFileKeys.XGnomeSingleWindowKey:
                    if (bool.TryParse(value, out var gnomeSingleWindow))
                        desktopFile.SingleMainWindow = gnomeSingleWindow;
                    break;
            }
        }

        if (desktopFile.Exec is null)
            desktopFile.IsBroken = true;

        return desktopFile;
    }

    private static bool IsLocalizedKey(ReadOnlySpan<char> key, string baseKey, CultureInfo userLanguage)
    {
        if (!key.StartsWith(baseKey.AsSpan()))
            return false;

        if (key.Length <= baseKey.Length || key[baseKey.Length] != '[')
            return false;

        var closingBracketIndex = key.IndexOf(']');
        if (closingBracketIndex == -1)
            return false;

        var languageCode = key.Slice(
            baseKey.Length + 1,
            closingBracketIndex - baseKey.Length - 1
        );

        if (languageCode.IsEmpty)
            return false;

        var normalizedUserLang = userLanguage.Name.Replace('-', '_');
        var desktopLang = languageCode.ToString();

        return desktopLang.Equals(normalizedUserLang, StringComparison.OrdinalIgnoreCase) ||
               desktopLang.Equals(userLanguage.TwoLetterISOLanguageName, StringComparison.OrdinalIgnoreCase);
    }
}