using FakeItEasy;
using GMenu.Modules.Configuration.Interfaces;
using GMenu.Modules.Configuration.Model;
using GMenu.Modules.DesktopFiles;
using GMenu.Modules.DesktopFiles.Interfaces;
using GMenu.Modules.DesktopFiles.Model;
using GMenu.Modules.LinuxSystem.interfaces;
using Serilog;
using TheMakarik.Testing.FileSystem;
using TheMakarik.Testing.FileSystem.AutoNaming;

public class DesktopFileHeaderReaderTests : IDisposable
{
    private IFileSystem _fileSystem;
    private readonly IDesktopFileHeaderReader _systemUnderTest;
    private readonly IConfigurationProvider _configurationProvider;

    public DesktopFileHeaderReaderTests()
    {
        _configurationProvider = A.Fake<IConfigurationProvider>();
        A.CallTo(() => _configurationProvider.CurrentObservable).ReturnsLazily(() => new ObservableConfiguration()
        {
            UnexistingCategories = [],
            User = null!,
            SearchDesktopFilesDirectories = [new DesktopFileDirectory(Path: _fileSystem!.Root, null)]
        });
        _systemUnderTest = new DesktopFileHeaderReader(A.Dummy<ILogger>(), _configurationProvider, A.Dummy<IRootRequirer>());
    }

    [Theory]
    [InlineData("1cv8t-8.3.27-1508", "Office", "1C")]
    public void ReadAllHeaders_WithNormalDesktopFile_MustReturnNormalHeader(string icon, string category,
        string name)
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @$"
[Desktop Entry]
Version=1.0
Type=Application
Terminal=false
Exec=/opt/1cv8t/x86_64/8.3.27.1508/1cv8t
Categories={category};Finance;
Name[ar]=8.3.27.1508\nالنسخة التعليمية\nالعميل السمين
Name[az]=8.3.27.1508\nTədris versiyası\nQalın kliyent
Name[bg]=8.3.27.1508\nУчебна версия\nКлиент
Name[de]=8.3.27.1508\nÜbungsversion\nDicker Client
Name[el]=8.3.27.1508\nΕκπαιδευτική έκδοση\nΠαχύς πελάτης
Name[en_GB]=8.3.27.1508\nTraining version\nThick client
Name[es]=8.3.27.1508\nVersión de entrenamiento\nCliente gordo
Name[fr]=8.3.27.1508\nVersion éducative\nClient lourd
Name[hu]=8.3.27.1508\nOktatási verzió\nVastag kliens
Name[hy]=8.3.27.1508\nՈւսումնական տարբերակ\nՀաստ հաճախորդ
Name[it]=8.3.27.1508\nVersione didattica\nThick client
Name[ka]=8.3.27.1508\nსასწავლო ვერსია\nმსხვილი კლიენტი
Name[kk]=8.3.27.1508\nОқыту нұсқасы\nЗор клиент
Name[lt]=8.3.27.1508\nMokomoji versija\nStoras klientas
Name[lv]=8.3.27.1508\nMācību versija\nThick Client
Name[pl]=8.3.27.1508\nWersja szkoleniowa\nGruby klient
Name[ro]=8.3.27.1508\nVersiunea de instruire\nStandard client
Name[ru_RU]=8.3.27.1508\nУчебная версия\nТолстый клиент
Name[tk]=8.3.27.1508\nBilim görnüşi\nSemiz müşderi
Name[tr]=8.3.27.1508\nEğitim sürümü\nKalın istemci
Name[uk]=8.3.27.1508\nУчбова версія\nТовстий клієнт
Name[vi]=8.3.27.1508\nPhiên bản học tập\nClient dày
Name[zh]=8.3.27.1508\n学生版\n胖客户端
Name={name}
Comment[ar]=تشغيل وضع 1C:Enterprise
Comment[az]=1C:Enterprise rejimində işə salın
Comment[bg]=Стартирай в режим 1C:Предприятие
Comment[de]=Im 1C:Enterprise-Modus starten
Comment[el]=Εκκίνηση σε λειτουργία του 1C:Enterprise
Comment[en_GB]=Run in 1C:Enterprise mode
Comment[es]=Lanzamiento en el modo de 1C:Empresa
Comment[fr]=Démarrage dans le mode 1C:Enterprise
Comment[hu]=Kezdve a mĂłdban 1C:Enterprise
Comment[hy]=Գործարկել 1C:Enterprise ռեժիմում
Comment[it]=Esecuzione in modalità 1C:Enterprise
Comment[ka]=გაშვება 1C:Enterprise რეჟიმში
Comment[kk]=1С:Кәсіпорын режимінде іске қосу
Comment[lt]=Paleisti 1C:Organizacija režime
Comment[lv]=Palaist režīmā 1C:Uzņēmums
Comment[pl]=Uruchomienie w trybie 1C:Enterprise
Comment[ro]=Lansați în 1C:Enterprise mod
Comment[ru]=Запуск в режиме 1С:Предприятия
Comment[tk]=1C:Kärhana re iniminde başlaň
Comment[tr]=1C:İşletme Biçiminde çalıştır
Comment[uk]=Запуск в режимі 1С:Підприємства
Comment[vi]=Khởi động trong chế độ 1C:Enterprise
Comment[zh]=在 1C:企业的模式上启动
Comment=Run in 1C:Enterprise mode
Icon={icon}
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders().First();
        //Assert
        Assert.Equal(result.Category, category);
        Assert.Equal(result.IconPath, icon);
        Assert.Equal(result.Name, name);
        Assert.Equal(result.Directory, _fileSystem.Root);
        Assert.False(result.IsBroken);
    }
    
    [Fact]
    public void ReadAllHeaders_WithoutDesktopEntryHeader_MustBeBroken()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @"
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
Keywords=2d;curses;colour;single-player;
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.True(result.First().IsBroken);
    }
    
    
    [Fact]
    public void ReadAllHeaders_OneWithoutDesktopEntryHeaderAndOneNormal_MustReturnOnlyValid()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var brokenFilePath, @"
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
Keywords=2d;curses;colour;single-player;
")
            .AddFileWithNameGeneraing(".desktop", out var path, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Icon=tint
Terminal=true
Type=Application
Categories=Game;BlocksGame;
Keywords=2d;curses;colour;single-player;
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.Equal(2, result.Count);
        Assert.True(result.FirstOrDefault(header => header.IsBroken) is not null);
    }
    
    [Fact]
    public void ReadAllHeaders_WithoutIcon_IconIsNullButNotBroken()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Terminal=true
Type=Application
Categories=Game;BlocksGame;
Keywords=2d;curses;colour;single-player;
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.Null(result.First().IconPath);
        Assert.False(result.First().IsBroken);
    }
    
    [Fact]
    public void ReadAllHeaders_WithoutCategory_CategoryIsNullButNotBroken()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @"
[Desktop Entry]
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Exec=sh -c '/usr/games/tint -l 1;echo;echo PRESS ENTER;read line'
Terminal=true
Type=Application
Icon=tint
Keywords=2d;curses;colour;single-player;
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.Null(result.First().Category);
        Assert.False(result.First().IsBroken);
    }
    
    [Fact]
    public void ReadAllHeaders_WithoutExec_MustBeBroken()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @"
[Desktop Entry]
Type=Application
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Terminal=true
Icon=tint
Keywords=2d;curses;colour;single-player;
")
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.True(result.First().IsBroken);
    }
    
    [Fact]
    public void ReadAllHeaders_SearchDesktopFilesRecursive()
    {
        //Arrange
        _fileSystem = FileSystem.BeginBuilding()
            .AddRandomRootName()
            .AddNameGenerator(NameGenerationType.RandomName)
            .AddFileWithNameGeneraing(".desktop", out var path, @"
[Desktop Entry]
Type=Application
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Terminal=true
Icon=tint
Exec=tint
Keywords=2d;curses;colour;single-player;
")
            .AddDirectoriesWithNameGenerating(3, (_, builder) => 
                builder.AddFilesWithNameGeneraing(".desktop", 5, @"
[Desktop Entry]
Type=Application
Name=TINT
Comment=Tetris clone for the terminal
Comment[es]=Un clon del Tetris para la terminal
Terminal=true
Icon=tint
Exec=tint
Keywords=2d;curses;colour;single-player;"))
            .Build();
        
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert 
        _fileSystem.Should().TotalFileCount(result.Count);
        Assert.All(result, header => Assert.False(header.IsBroken));
    }
    
    [Fact]
    public void ReadAllHeaders_UnexistedCategory_ReturnsAsDummyHeader()
    {
        //Arrange
        A.CallTo(() => _configurationProvider.CurrentObservable).Returns(new ObservableConfiguration()
        {
            SearchDesktopFilesDirectories = [],
            User = null!,
            UnexistingCategories = [new UnexistingCategory() { Name = "mockName", Path = "mockPath/mockName" }]
        });
        //Act 
        var result = _systemUnderTest.GetAllHeaders();
        //Assert
        Assert.True(result.First().IsDummy);
        Assert.False(result.First().IsBroken);
    }

    public void Dispose()
    {
        _fileSystem.Dispose();
    }
}