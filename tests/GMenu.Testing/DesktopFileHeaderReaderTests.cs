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

public class DesktopFileHeaderReaderTests : IDisposable
{
    private IFileSystem _fileSystem = null!;
    private readonly IDesktopFileHeaderReader _systemUnderTest;
    private readonly IConfigurationProvider _configurationProvider;
    private readonly IDesktopFileReader _desktopFileReader;

    public DesktopFileHeaderReaderTests()
    {
        _configurationProvider = A.Fake<IConfigurationProvider>();
        _desktopFileReader = new DesktopFileReader();
        
        A.CallTo(() => _configurationProvider.CurrentObservable).ReturnsLazily(() => new ObservableConfiguration()
        {
            UnexistingCategories = [],
            Language = CultureInfo.CurrentCulture,
            LocalizeDesktopFileNames = false
        });
        
        _systemUnderTest = new DesktopFileHeaderReader(
            _configurationProvider, 
            _desktopFileReader, 
            A.Dummy<ILogger>());
    }

    [Theory]
    [InlineData("1cv8t-8.3.27-1508", "Office", "1C")]
    public void ReadAllHeaders_WithNormalDesktopFile_MustReturnNormalHeader(string icon, string category, string name)
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
        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths).First();

        // Assert
        Assert.Equal(category, result.Category);
        Assert.Equal(icon, result.IconPath);
        Assert.Equal(name, result.Name);
        Assert.Equal(filePath, result.Path);
        Assert.False(result.IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithoutDesktopEntryHeader_MustBeBroken()
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

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.True(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_OneWithoutDesktopEntryHeaderAndOneNormal_MustReturnBoth()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var brokenFileName, @"
Name=TINT
Comment=Tetris clone for the terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
")
            .AddFileWithNameGeneraing(".desktop", out var normalFileName, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.FirstOrDefault(header => header.IsBroken) is not null);
    }

    [Fact]
    public void ReadAllHeaders_WithoutIcon_IconIsNullButNotBroken()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Terminal=true
Type=Application
Categories=Game;BlocksGame;
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Null(result.First().IconPath);
        Assert.False(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithoutCategory_CategoryIsNullButNotBroken()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Terminal=true
Type=Application
Icon=tint
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Null(result.First().Category);
        Assert.False(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithoutExec_MustBeBroken()
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

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.True(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_SearchDesktopFilesRecursive()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out _, @"
[Desktop Entry]
Type=Application
Name=TINT
Exec=tint
Terminal=true
Icon=tint
")
            .AddDirectoriesWithNameGenerating(3, (_, builder) => 
                builder.AddFilesWithNameGenerating(".desktop", 5, @"
[Desktop Entry]
Type=Application
Name=TINT
Exec=tint
Terminal=true
Icon=tint"))
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert 
        _fileSystem.Should().TotalFileCount(result.Count);
        Assert.All(result, header => Assert.False(header.IsBroken));
    }

    [Fact]
    public void ReadAllHeaders_WithLocalizedName_WhenLocalizationEnabled_MustUseLocalizedName()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=Default Name
Name[ru]=Русское Имя
Exec=/usr/bin/app
Icon=app-icon
Categories=Utility;
")
            .Build();

        // Setup configuration with Russian language and localization enabled
        A.CallTo(() => _configurationProvider.CurrentObservable).Returns(new ObservableConfiguration()
        {
            UnexistingCategories = [],
            Language = new CultureInfo("ru-RU"),
            LocalizeDesktopFileNames = true
        });

        var systemUnderTest = new DesktopFileHeaderReader(
            _configurationProvider, 
            _desktopFileReader, 
            A.Dummy<ILogger>());

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Equal("Русское Имя", result.First().Name);
        Assert.Equal("Name[ru]", result.First().NameKey);
        Assert.False(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithLocalizedName_WhenLocalizationDisabled_MustUseDefaultName()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=Default Name
Name[ru]=Русское Имя
Exec=/usr/bin/app
Icon=app-icon
Categories=Utility;
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Equal("Default Name", result.First().Name);
        Assert.Equal("Name", result.First().NameKey);
        Assert.False(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithMultiplePaths_ShouldSearchAllDirectories()
    {
        // Arrange
        var fs1 = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out _, @"[Desktop Entry]
Name=App1
Exec=app1
Icon=icon1")
            .Build();

        var fs2 = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out _, @"[Desktop Entry]
Name=App2
Exec=app2
Icon=icon2")
            .Build();

        var paths = new[] { fs1.Root, fs2.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Contains(result, h => h.Name == "App1");
        Assert.Contains(result, h => h.Name == "App2");

        fs1.Dispose();
        fs2.Dispose();
    }

    [Fact]
    public void ReadAllHeaders_WithSpecialCharactersInName_ShouldReplaceNewline()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=App\nWith\nNewlines
Exec=/usr/bin/app
Icon=app-icon
Categories=Utility;
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.Equal("App With Newlines", result.First().Name);
    }

    [Fact]
    public void ReadAllHeaders_WithNoDisplayTrue_ShouldMarkAsHidden()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=Hidden App
Exec=/usr/bin/hidden-app
Icon=hidden-icon
Categories=Utility;
NoDisplay=true
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.True(result.First().IsHidden);
        Assert.False(result.First().IsBroken);
    }

    [Fact]
    public void ReadAllHeaders_WithNoDisplayFalse_ShouldNotMarkAsHidden()
    {
        // Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var fileName, @"
[Desktop Entry]
Name=Visible App
Exec=/usr/bin/visible-app
Icon=visible-icon
Categories=Utility;
NoDisplay=false
")
            .Build();

        var paths = new[] { _fileSystem.Root };

        // Act 
        var result = _systemUnderTest.GetAllHeaders(paths);

        // Assert
        Assert.False(result.First().IsHidden);
        Assert.False(result.First().IsBroken);
    }

    public void Dispose()
    {
        _fileSystem?.Dispose();
    }
}