
using System.Globalization;
using FakeItEasy;
using GMenu.Modules.Configuration.Interfaces;
using GMenu.Modules.Configuration.Model;
using GMenu.Modules.DesktopFiles;
using GMenu.Modules.DesktopFiles.Interfaces;
using Serilog;
using TheMakarik.Testing.FileSystem;
using TheMakarik.Testing.FileSystem.AutoNaming;

namespace GMenu.Testing;

public class DesktopFileEntityReaderTests : IDisposable
{
    private IFileSystem _fileSystem = null!;
    private readonly IDesktopFileEntityReader _systemUnderTest;
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IDesktopFileReader _desktopFileReader;

    public DesktopFileEntityReaderTests()
    {
        _configurationProvider = A.Fake<IConfigurationProvider>();
        _desktopFileReader = new DesktopFileReader();
        
        A.CallTo(() => _configurationProvider.CurrentObservable).ReturnsLazily(() => new ObservableConfiguration
        {
            CustomCategories = [],
            Language = CultureInfo.CurrentCulture,
            LocalizeDesktopFiles = false,
            Version = new Version()
        });
        
        _systemUnderTest = new DesktopFileEntityReader(
            _configurationProvider, 
            _desktopFileReader, 
            A.Dummy<ILogger>());
    }

    [Theory]
    [InlineData("1cv8t-8.3.27-1508", "Office", "1C")]
    public async Task ReadDesktopFileAsync_WithNormalDesktopFile_MustReturnFullDesktopFile(string icon, string category, string name)
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, $@"
[Desktop Entry]
Version=1.0
Type=Application
Terminal=false
Exec=/opt/1cv8t/x86_64/8.3.27.1508/1cv8t
Categories={category};Finance;
Name={name}
Icon={icon}
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(category, result.Category);
        Assert.Equal($"{category};Finance;", result.Categories);
        Assert.Equal(icon, result.IconPath);
        Assert.Equal(name, result.Name);
        Assert.Equal(filePath, result.Path);
        Assert.Equal("1.0", result.Version);
        Assert.False(result.Terminal);
        Assert.Equal("/opt/1cv8t/x86_64/8.3.27.1508/1cv8t", result.Exec);
        Assert.False(result.IsBroken);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithoutDesktopEntryHeader_MustBeBroken()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
Name=TINT
Comment=Tetris clone for the terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsBroken);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithoutExec_MustBeBroken()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Type=Application
Name=TINT
Comment=Tetris clone for the terminal
Terminal=true
Icon=tint
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.IsBroken);
        Assert.Null(result.Exec);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithAllBooleanFlags_MustParseCorrectly()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=app
NoDisplay=true
Hidden=true
Terminal=true
DBusActivatable=true
StartupNotify=true
SingleMainWindow=true
X-GNOME-AutoRestart=true
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.NoDisplay);
        Assert.True(result.Hidden);
        Assert.True(result.Terminal);
        Assert.True(result.DBusActivatable);
        Assert.True(result.StartupNotify);
        Assert.True(result.SingleMainWindow);
        Assert.True(result.XGnomeAutoRestart);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithStringFields_MustParseCorrectly()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=icon
StartupWMClass=MyApp
MimeType=text/plain;image/png;
Implements=org.freedesktop.FileManager1
TryExec=/usr/bin/check
OnlyShowIn=GNOME;Unity;
NotShowIn=KDE;
Version=1.2
X-GNOME-UsesNotifications=true
Path=/home/user/work
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("MyApp", result.StartupWmClass);
        Assert.Equal("text/plain;image/png;", result.MimeType);
        Assert.Equal("org.freedesktop.FileManager1", result.Implements);
        Assert.Equal("/usr/bin/check", result.TryExec);
        Assert.Equal("GNOME;Unity;", result.OnlyShowIn);
        Assert.Equal("KDE;", result.NotShowIn);
        Assert.Equal("1.2", result.Version);
        Assert.Equal("true", result.XGnomeUsesNotifications);
        Assert.Equal("/home/user/work", result.WorkingDirectory);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithXGnomeSingleWindow_ShouldSetSingleMainWindow()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=icon
X-GNOME-SingleWindow=true
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.SingleMainWindow);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithCategories_MustParseFirstCategoryAndFullCategories()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=icon
Categories=Development;IDE;Java;
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Development", result.Category);
        Assert.Equal("Development;IDE;Java;", result.Categories);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithSpecialCharactersInName_ShouldReplaceNewline()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App\nWith\nNewlines
Icon=icon
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("App With Newlines", result.Name);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithGenericNameAndComment_ShouldParse()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=icon
GenericName=Text Editor
Comment=Edit text files
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Text Editor", result.GenericName);
        Assert.Equal("Edit text files", result.Comment);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WithKeywords_ShouldParse()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Exec=/usr/bin/app
Name=App
Icon=icon
Keywords=editor;word;document;
")
            .Build();

        var filePath = Path.ChangeExtension(fileName, ".desktop");

        // Act 
        var result = await _systemUnderTest.ReadDesktopFileAsync(filePath);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("editor;word;document;", result.Keywords);
    }

    [Fact]
    public async Task ReadDesktopFileAsync_WhenFileDoesNotExist_ReturnsNull()
    {
        // Act
        var result = await _systemUnderTest.ReadDesktopFileAsync("/nonexistent/file.desktop");

        // Assert
        Assert.Null(result);
    }

    public void Dispose()
    {
        _fileSystem?.Dispose();
    }
}