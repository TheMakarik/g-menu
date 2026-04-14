using BenchmarkDotNet.Attributes;
using GMenu.Models.DesktopFiles;
using GMenu.Modules.Configuration.Model;
using GMenu.Modules.DesktopFiles.Model;
using Serilog;
using ZLinq;

namespace GMenu.Benchmarks;

[MemoryDiagnoser]
public class ReadDesktopFilesHeaderBenchmarkWithMoreThan200Files
{
    private ObservableConfiguration _configuration;
    private ILogger _logger = new LoggerConfiguration().CreateLogger();
    private const string PathToFile = "test/benchmark";
    private const string DesktopEntryHeader = "[Desktop Entry]";
    private const string ExecKey = "Exec";
    private const string NameKey = "Name";
    private const string IconKey = "Icon";
    private const string CategoriesKey = "Categories";
    private const string NoDisplayKey = "NoDisplay";
    private const int FilesCount = 250;

    [GlobalSetup]
    public void Setup()
    {
        var directory = Directory.CreateDirectory(Path.GetDirectoryName(PathToFile)!);
        for (var i = 0; i < FilesCount; i++)
        {
            using var stream = File.Create(PathToFile + i.ToString() + ".desktop");
            using var writer = new StreamWriter(stream);
            writer.Write(@"
[Desktop Entry]
Type=Application
Exec=lxqt-config-appearance
Icon=preferences-desktop-theme
Categories=Settings;DesktopSettings;Qt;LXQt;
OnlyShowIn=LXQt;

Name=Appearance
GenericName=Appearance Settings
Comment=Configure theme, colors, font, icons and more
Comment[ar]=قم باعداد المظهر والألوان والخط والأيقونات والمزيد
GenericName[ar]=إعدادات المظهر
Name[ar]=المظهر
Name[bg]=Външен изглед
GenericName[bg]=Настройки на външен изглед
Comment[bg]=Настройване на външен изглед: тема, цвят, шрифт, икони и други настройки
Name[ca]=Aparença
GenericName[ca]=Ajusts de l'aparença
Comment[ca]=Configureu el tema, els colors, el tipus de lletra, les icones i molt més
Comment[cs]=Nastavení vzhledu pro LXQt
GenericName[cs]=Nastavení vzhledu
Name[cs]=Vzhled
Name[da]=Udseende
GenericName[da]=Indstillinger for udseende
Comment[da]=Indstil tema, farver, skrifttype, ikoner og mere
Name[de]=Erscheinungsbild
GenericName[de]=Erscheinungsbild-Einstellungen
Comment[de]=Konfiguration von Thema, Schriftart, Symbole und mehr
Comment[el]=Διαμόρφωση θέματος, χρωμάτων, γραμματοσειράς, εικονιδίων και άλλων
GenericName[el]=Ρυθμίσεις εμφάνισης
Name[el]=Εμφάνιση
Comment[eo]=Agordi aperon de LXQt-labortablo
GenericName[eo]=Agordoj de apero de LXQt
Name[eo]=Agordoj de apero de LXQt
Name[es]=Aspecto
GenericName[es]=Configuración de la Apariencia
Comment[es]=Configurar tema, colores, fuentes, íconos y más
Comment[es_VE]=Configurar apariencia del escritorio LXQt
GenericName[es_VE]=Configuración de apariencia de LXQt
Name[es_VE]=Configuración de apariencia de LXQt
Name[et]=Välimus
GenericName[et]=LXQt välimuse seadistused
Comment[et]=Halda LXQt kujundust, värve, kirjatüüpe, ikoone ja palju muud
Comment[eu]=Konfiguratu LXQt mahaigainaren itxura
GenericName[eu]=LXQt itxuraren konfigurazioa
Name[eu]=LXQt itxuraren konfigurazioa
Comment[fi]=Määritä teema, värit, fontti, kuvakkeet ja paljon muuta
GenericName[fi]=Ulkoasun hallinta
Name[fi]=LXQt:n ulkoasun hallinta
Name[fr]=Apparence
GenericName[fr]=Paramètres d'apparence
Comment[fr]=Configurer le thème, les couleurs, la police, les icônes et plus encore
Name[gl]=Aparencia
GenericName[gl]=Configuración da aparencia
Comment[gl]=Configuración da aparencia do LXQt
Name[he]=מראה
GenericName[he]=הגדרות מראה
Comment[he]=הגדרת ערכת עיצוב, צבעים, גופן, סמלים ועוד
Name[hr]=Izgled
GenericName[hr]=Postavke izgleda
Comment[hr]=Konfiguriraj temu, boje, font, ikone i još više
Name[hu]=Megjelenés
GenericName[hu]=Megjelenés beállítása
Comment[hu]=Az LXQt megjelenésének beállítása
Name[it]=Aspetto
GenericName[it]=Configurazione dell'aspetto di LXQt
Comment[it]=Configura tema, colori, caratteri e altro
Name[ja]=外観
GenericName[ja]=外観の設定
Comment[ja]=テーマ、色、フォント、アイコン等を設定します
Name[ka]=გარეგნობა
GenericName[ka]=გარეგნობის მორგება
Comment[ka]=მოირგეთ თემა, ფერები, ფონტი, ხატულები და ა.შ
Name[ko]=모양새
GenericName[ko]=모양새 설정
Comment[ko]=테마, 색상, 글꼴, 아이콘 등을 구성합니다
Name[lg]=Endabika
GenericName[lg]=Enteekateeka za ndabika
Comment[lg]=Teekateeka lulyo lw'endabika, langi, enkula y'ennukuta, bufaananyi obuyunzi n'ebirala
Comment[lt]=Konfigūruoti apipavidalinimą, spalvas, šriftą, piktogramas ir kt.
GenericName[lt]=Išvaizdos nustatymai
Name[lt]=Išvaizda
Name[nb_NO]=Utseende
GenericName[nb_NO]=Utseendeinnstillinger
Comment[nb_NO]=Innstillinger for tema, farger, skrift, ikoner og mer
Comment[nl]=Pas het onder andere het thema, lettertype en kleuren van LXQt aan
GenericName[nl]=Vormgevingsinstellingen
Name[nl]=Vormgeving
Name[oc]=Aparéncia
GenericName[oc]=Paramètres de l’aparéncia
Comment[oc]=Configurar lo tèma, las colors, la polissa, las icònas e encara mai
Comment[pl_PL]=Konfigurowanie motywu, koloru, czcionki, ikony i innych
GenericName[pl_PL]=Ustawienia wyglądu
Name[pl_PL]=Konfiguracja wyglądu LXQt
Name[pt]=Aparência
GenericName[pt]=Definições de Aparência
Comment[pt]=Configurar o tema, as cores, o tipo de letra, os ícones e muito mais
Comment[pt_BR]=Configurar tema, cores, fonte, ícones e mais
GenericName[pt_BR]=Configurações de Aparência
Name[pt_BR]=Aparência
Comment[ro_RO]=Configurează aspectul desktopului LXQt
GenericName[ro_RO]=Configurare aspect LXQt
Name[ro_RO]=Configurare aspect LXQt
Comment[ru]=Настраивайте темы, цвета, шрифты, значки и другое
GenericName[ru]=Настройка внешнего вида
Name[ru]=Внешний вид
Comment[sk]=Nastavenie témy, farby, písma, ikôn a mnoho ďalšieho
GenericName[sk]=Nastavenie vzhľadu
Name[sk]=Vzhľad
Comment[sl]=Nastavite videz namizja LXQt
GenericName[sl]=Nastavitev videza
Name[sl]=Nastavitev videza namizja LXQt
Comment[sr]=Подесите изглед Рејзорове радне површи
GenericName[sr]=Подешавање изгледа
Name[sr]=Подешавање изгледа Рејзора
Name[sr@ijekavian]=Подешавање изгледа Рејзора
Comment[sr@ijekavian]=Подесите изглед Рејзорове радне површи
GenericName[sr@ijekavian]=Подешавање изгледа
Name[sr@ijekavianlatin]=Podešavanje izgleda Rejzora
Comment[sr@ijekavianlatin]=Podesite izgled Rejzorove radne površi
GenericName[sr@ijekavianlatin]=Podešavanje izgleda
Comment[sr@latin]=Podesite izgled Rejzorove radne površi
GenericName[sr@latin]=Podešavanje izgleda
Name[sr@latin]=Podešavanje izgleda Rejzora
Comment[th_TH]=ตั้งค่ารูปลักษณ์ของเดสก์ท็อป LXQt
GenericName[th_TH]=การตั้งค่ารูปลักษณ์ LXQt
Name[th_TH]=การตั้งค่ารูปลักษณ์ LXQt
Comment[tr]=Temayı, renkleri, yazı tipini, simgeleri ve daha fazlasını yapılandırın
GenericName[tr]=Görünüm Ayarları
Name[tr]=Görünüm
Comment[uk]=Налаштування теми, кольорів, шрифту, піктограм та іншого
GenericName[uk]=Налаштування вигляду
Name[uk]=Налаштування вигляду LXQt
Name[vi]=Hình thức
GenericName[vi]=Cài đặt Hình thức
Comment[vi]=Cài đặt Hình thức cho LXQt
Comment[zh_CN]=配置 LXQt 的外观
GenericName[zh_CN]=外观配置
Name[zh_CN]=外观
Comment[zh_TW]=自定LXQt桌面的外觀
GenericName[zh_TW]=LXQt外觀設定
Name[zh_TW]=LXQt外觀設定
");
        }

        _configuration = new ObservableConfiguration
        {
            SearchDesktopFilesDirectories = [new DesktopFileDirectory(directory.FullName, null)],
            UnexistingCategories = []
        };
    }

    [Benchmark(Baseline = true)]
    public void ReadDesktopFilesUsingZLinq()
    {
        var result = _configuration.SearchDesktopFilesDirectories
            .AsValueEnumerable()
            .Select(directory => directory.Path)
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }))
            .Select(path =>
                File.ReadLines(path)
                    .AsValueEnumerable()
                    .Where(line => !string.IsNullOrWhiteSpace(line))
                    .SkipWhile(line => !line.Equals(DesktopEntryHeader))
                    .Skip(1) //Skip the line with the header
                    .TakeWhile(line => line[0] != '[')
                    .Where(line => line.StartsWith(NameKey)
                                   || line.StartsWith(IconKey)
                                   || line.StartsWith(CategoriesKey)
                                   || line.StartsWith(NoDisplayKey)
                                   || line.StartsWith(ExecKey))
                    .Select(line => (Line: line, EqualsIndex: line.IndexOf('=')))
                    .Where(halfParsedString =>
                        halfParsedString.EqualsIndex != -1) //-1 if IndexOf(..) cannot find character
                    .Where(halfParsedString => !halfParsedString
                        .Line
                        .AsSpan(0, halfParsedString.EqualsIndex).Contains('[')) //skip localized values
                    .Aggregate(new DesktopFileHeader { Directory = Path.GetDirectoryName(path)! },
                        (header, halfParsedString) =>
                        {
                            var value = halfParsedString.Line.AsSpan(halfParsedString.EqualsIndex + 1);
                            var key = halfParsedString.Line.AsSpan(0, halfParsedString.EqualsIndex);

                            switch (key)
                            {
                                case NameKey:
                                    header.Name = value.ToString();
                                    break;
                                case IconKey:
                                    header.IconPath = value.ToString();
                                    break;
                                case CategoriesKey:
                                    var semicolonIndex = value.IndexOf(';');
                                    header.Category = semicolonIndex == -1
                                        ? value.ToString()
                                        : value[..semicolonIndex].ToString();
                                    break;
                                case ExecKey:
                                    header.Exec = value.ToString();
                                    break;
                                case NoDisplayKey:
                                    if (bool.TryParse(value, out var isHidden))
                                        header.IsHidden = isHidden;
                                    else
                                        _logger.Error("One of desktop files in {path} has corrupted NoDisplay value",
                                            path);

                                    break;
                            }

                            return header;
                        })
            )
            .OrderBy(header => header.Directory)
            .ThenBy(header => header.Category)
            .ToList();
    }

    [Benchmark]
    public async Task ReadDesktopFilesByFrolotey1()
    {
        var cancellationTokenSource = new CancellationTokenSource();
        var searchDirectories = _configuration.SearchDesktopFilesDirectories;
        var unexistingCategories = _configuration.UnexistingCategories;

        var allFilePaths = searchDirectories
            .SelectMany(directory => GetDesktopFilesRecursively(directory.Path))
            .ToList();

        var parsedHeaders = new List<DesktopFileHeader?>();
        foreach (var filePath in allFilePaths)
        {
            cancellationTokenSource.Token.ThrowIfCancellationRequested();
            var header = await ParseDesktopFileAsync(filePath, cancellationTokenSource.Token);
            parsedHeaders.Add(header);
        }

        var allHeaders = parsedHeaders
            .Where(header => header != null)
            .Select(header => header!)
            .Concat(unexistingCategories.Select(category => new DesktopFileHeader
            {
                Directory = category.Path,
                Category = category.Name,
                IsDummy = true,
                IsHidden = false
            }))
            .OrderBy(header => header.Directory)
            .ThenBy(header => header.Category ?? string.Empty)
            .ToList();
    }

    [Benchmark]
    public void ReadDesktopFilesMyRealizationNoZLinq()
    {
        var filesToHandle = _configuration.SearchDesktopFilesDirectories
            .Select(directory => directory.Path)
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }));

        var result = filesToHandle.Select<string, DesktopFileHeader?>(filePath =>
            {
                GetDesktopFileLineEnumeration(out var lines, filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Directory = Path.GetDirectoryName(filePath)!,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                };
                var wasFoundNoDisplay = false;
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;


                    var equalsIndex = line.IndexOf('=');

                    if (equalsIndex == -1)
                    {
                        _logger.Warning("Cannot find '=' in the line {line} of the desktop file: {file}", line,
                            filePath);
                        continue;
                    }


                    var key = line.AsSpan(0, equalsIndex);
                    var value = line.AsSpan(equalsIndex + 1);

                    if (key[^1] == ']')
                        continue;

                    switch (key)
                    {
                        case NameKey:
                            desktopFileHeader.Name = value.ToString();
                            break;
                        case IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case NoDisplayKey:
                            if (bool.TryParse(value, out var isHidden))
                                desktopFileHeader.IsHidden = isHidden;
                            else
                                _logger.Error("Desktop file: {path} has corrupted NoDisplay value", filePath);
                            break;
                    }

                    if (DesktopFileHeaderIsReady(desktopFileHeader, wasFoundNoDisplay))
                        break;
                }

                return CanReturnDesktopFile(desktopFileHeader)
                    ? desktopFileHeader
                    : null;
            })
            .Where(desktopFileHeader => desktopFileHeader is not null)
            .Cast<DesktopFileHeader>()
            .ToList();
    }
    
      [Benchmark]
    public void ReadDesktopFilesMyRealizationParallel()
    {
        var filesToHandle = _configuration.SearchDesktopFilesDirectories
            .Select(directory => directory.Path)
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }));

        var result = filesToHandle
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Select<string, DesktopFileHeader?>(filePath =>
            {
                GetDesktopFileLineEnumeration(out var lines, filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Directory = Path.GetDirectoryName(filePath)!,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                };
                var wasFoundNoDisplay = false;
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;


                    var equalsIndex = line.IndexOf('=');

                    if (equalsIndex == -1)
                    {
                        _logger.Warning("Cannot find '=' in the line {line} of the desktop file: {file}", line,
                            filePath);
                        continue;
                    }


                    var key = line.AsSpan(0, equalsIndex);
                    var value = line.AsSpan(equalsIndex + 1);

                    if (key[^1] == ']')
                        continue;

                    switch (key)
                    {
                        case NameKey:
                            desktopFileHeader.Name = value.ToString();
                            break;
                        case IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case NoDisplayKey:
                            if (bool.TryParse(value, out var isHidden))
                                desktopFileHeader.IsHidden = isHidden;
                            else
                                _logger.Error("Desktop file: {path} has corrupted NoDisplay value", filePath);
                            break;
                    }

                    if (DesktopFileHeaderIsReady(desktopFileHeader, wasFoundNoDisplay))
                        break;
                }

                return CanReturnDesktopFile(desktopFileHeader)
                    ? desktopFileHeader
                    : null;
            })
            .Where(desktopFileHeader => desktopFileHeader is not null)
            .Cast<DesktopFileHeader>()
            .ToList();
    }

    [Benchmark]
    public void ReadDesktopFilesMyRealizationWithZLinq()
    {
        var filesToHandle = _configuration.SearchDesktopFilesDirectories
            .Select(directory => directory.Path)
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }));

        var result = filesToHandle
            .AsValueEnumerable()
            .Select(filePath =>
            {
                GetDesktopFileLineEnumeration(out var lines, filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Directory = Path.GetDirectoryName(filePath)!,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                };
                var wasFoundNoDisplay = false;
                foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;


                    var equalsIndex = line.IndexOf('=');

                    if (equalsIndex == -1)
                    {
                        _logger.Warning("Cannot find '=' in the line {line} of the desktop file: {file}", line,
                            filePath);
                        continue;
                    }


                    var key = line.AsSpan(0, equalsIndex);
                    var value = line.AsSpan(equalsIndex + 1);

                    if (key[^1] == ']')
                        continue;

                    switch (key)
                    {
                        case NameKey:
                            desktopFileHeader.Name = value.ToString();
                            break;
                        case IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case NoDisplayKey:
                            if (bool.TryParse(value, out var isHidden))
                                desktopFileHeader.IsHidden = isHidden;
                            else
                                _logger.Error("Desktop file: {path} has corrupted NoDisplay value", filePath);
                            break;
                    }

                    if (DesktopFileHeaderIsReady(desktopFileHeader, wasFoundNoDisplay))
                        break;
                }

                return CanReturnDesktopFile(desktopFileHeader)
                    ? desktopFileHeader
                    : null;
            })
            .Where(desktopFileHeader => desktopFileHeader is not null)
            .Cast<DesktopFileHeader>()
            .ToList();
    }

    [Benchmark]
    public async Task ReadDesktopFilesMyRealizationWithAsync()
    {
        var filesToHandle = _configuration.SearchDesktopFilesDirectories
            .Select(directory => directory.Path)
            .SelectMany(directory => Directory
                .EnumerateFiles(directory, "*.desktop", new EnumerationOptions() { RecurseSubdirectories = true }));

        var result = await filesToHandle
            .ToAsyncEnumerable()
            .Select(async (filePath, token, task) =>
            {
                var lines = File.ReadLinesAsync(filePath);

                var desktopFileHeader = new DesktopFileHeader()
                {
                    Directory = Path.GetDirectoryName(filePath)!,
                    Name = Path.GetFileNameWithoutExtension(filePath),
                };
                var wasFoundNoDisplay = false;
                await foreach (var line in lines)
                {
                    if (string.IsNullOrEmpty(line))
                        continue;


                    var equalsIndex = line.IndexOf('=');

                    if (equalsIndex == -1)
                    {
                        _logger.Warning("Cannot find '=' in the line {line} of the desktop file: {file}", line,
                            filePath);
                        continue;
                    }


                    var key = line.AsSpan(0, equalsIndex);
                    var value = line.AsSpan(equalsIndex + 1);

                    if (key[^1] == ']')
                        continue;

                    switch (key)
                    {
                        case NameKey:
                            desktopFileHeader.Name = value.ToString();
                            break;
                        case IconKey:
                            desktopFileHeader.IconPath = value.ToString();
                            break;
                        case CategoriesKey:
                            var semicolonIndex = value.IndexOf(';');
                            desktopFileHeader.Category = semicolonIndex == -1
                                ? value.ToString()
                                : value[..semicolonIndex].ToString();
                            break;
                        case ExecKey:
                            desktopFileHeader.Exec = value.ToString();
                            break;
                        case NoDisplayKey:
                            if (bool.TryParse(value, out var isHidden))
                                desktopFileHeader.IsHidden = isHidden;
                            else
                                _logger.Error("Desktop file: {path} has corrupted NoDisplay value", filePath);
                            break;
                    }

                    if (DesktopFileHeaderIsReady(desktopFileHeader, wasFoundNoDisplay))
                        break;
                }

                return CanReturnDesktopFile(desktopFileHeader)
                    ? desktopFileHeader
                    : null;
            })
            .Where(async (header, token, task) => header is not null)
            .Cast<DesktopFileHeader>()
            .ToListAsync();
    }


    [GlobalCleanup]
    public void Cleanup()
    {
        Directory.Delete(Path.GetDirectoryName(PathToFile)!, true);
    }

    private (string Key, string Value) ParseDesktopLine(string line)
    {
        var equalIndex = line.IndexOf('=');
        if (equalIndex <= 0)
        {
            return (string.Empty, string.Empty);
        }

        var key = line[..equalIndex].Trim();
        var value = line[(equalIndex + 1)..].Trim();

        return (key, value);
    }

    private async Task<DesktopFileHeader?> ParseDesktopFileAsync(
        string filePath,
        CancellationToken cancellationToken)
    {
        try
        {
            var lines = await File.ReadAllLinesAsync(filePath, cancellationToken);
            var isInsideDesktopEntry = false;
            var header = new DesktopFileHeader
            {
                Directory = Path.GetDirectoryName(filePath) ?? string.Empty,
                IsHidden = false
            };
            var hasExec = false;
            var hasName = false;

            foreach (var rawLine in lines)
            {
                cancellationToken.ThrowIfCancellationRequested();
                var line = rawLine.Trim();

                if (line.StartsWith('['))
                {
                    if (isInsideDesktopEntry)
                    {
                        break;
                    }

                    if (line.Equals(DesktopEntryHeader, StringComparison.OrdinalIgnoreCase))
                    {
                        isInsideDesktopEntry = true;
                    }

                    continue;
                }

                if (!isInsideDesktopEntry || string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var (key, value) = ParseDesktopLine(line);

                if (key == ExecKey)
                {
                    hasExec = true;
                }
                else if (key == NameKey)
                {
                    header.Name = value;
                    hasName = true;
                }
                else if (key == IconKey)
                {
                    header.IconPath = value;
                }
                else if (key == CategoriesKey)
                {
                    header.Category = value.Split(';', StringSplitOptions.RemoveEmptyEntries).FirstOrDefault();
                }
                else if (key == NoDisplayKey)
                {
                    header.IsHidden = value.Equals(bool.TrueString, StringComparison.OrdinalIgnoreCase);
                }

                if (hasExec && hasName && header.Category != null)
                {
                    break;
                }
            }

            if (!isInsideDesktopEntry)
            {
                _logger.Warning("Missing [Desktop Entry] in: {FilePath}", filePath);
                return null;
            }

            if (!hasName)
            {
                _logger.Warning("Missing Name in: {FilePath}", filePath);
                return null;
            }

            if (!hasExec)
            {
                _logger.Warning("Missing Exec in: {FilePath}", filePath);
                return null;
            }

            return header;
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            _logger.Error(ex, "Error parsing: {FilePath}", filePath);
            return null;
        }
    }


    private IEnumerable<string> GetDesktopFilesRecursively(string directory)
    {
        if (!Directory.Exists(directory))
        {
            _logger.Warning("Directory not found: {Directory}", directory);
            yield break;
        }

        foreach (var file in Directory.GetFiles(directory, "*.desktop"))
        {
            yield return file;
        }

        foreach (var subDir in Directory.GetDirectories(directory))
        {
            foreach (var file in GetDesktopFilesRecursively(subDir))
            {
                yield return file;
            }
        }
    }

    private static bool CanReturnDesktopFile(DesktopFileHeader desktopFileHeader)
    {
        return desktopFileHeader.Name is not null && desktopFileHeader.Exec is not null;
    }

    private static bool DesktopFileHeaderIsReady(DesktopFileHeader desktopFileHeader, bool wasFoundNoDisplay)
    {
        return desktopFileHeader.Exec is not null
               && desktopFileHeader.IconPath is not null
               && desktopFileHeader.Name is not null
               && desktopFileHeader.Category is not null
               && wasFoundNoDisplay;
    }

    private static void GetDesktopFileLineEnumeration(out IEnumerable<string> lines, string filePath)
    {
        lines = File.ReadLines(filePath)
            .SkipWhile(line => line != DesktopEntryHeader)
            .Where(line => !string.IsNullOrEmpty(line))
            .Skip(1) // skip entry header
            .TakeWhile(line => line[0] is not '[' );
    }
}