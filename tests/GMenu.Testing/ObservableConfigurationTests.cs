using FakeItEasy;
using GMenu.Modules.Configuration.Model;
using GMenu.Modules.DesktopFiles.Model;

namespace GMenu.Testing;

public class ObservableConfigurationTests
{
   [Fact]
   public void AllProperties_WithNullValues_MustNoRaiseINotifyPropertyChanged()
   {
      //Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() {User = null!, SearchDesktopFilesDirectories = null!, UnexistingCategories = null!};
      observableConfiguration.PropertyChanged += (sender, args) => { wasPropertyChangedCalled = true; };
      //Act
      observableConfiguration.SearchDesktopFilesDirectories = [];
      //Assert
      Assert.False(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void AllProperties_WithNotNullValues_MustRaiseINotifyPropertyChanged()
   {
      //Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() {User = new  User(), SearchDesktopFilesDirectories = [], UnexistingCategories = []};
      observableConfiguration.PropertyChanged += (sender, args) => { wasPropertyChangedCalled = true; };
      //Act
      observableConfiguration.SearchDesktopFilesDirectories = [new DesktopFileDirectory(string.Empty, null)];
      //Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void ObservableCollection_OnValueChanging_MustRaiseINotifyPropertyChangedWithIndex()
   {
      //Arrange
      var propertyName = string.Empty;
      var observableConfiguration = new ObservableConfiguration() {User = new  User(), SearchDesktopFilesDirectories = [], UnexistingCategories = []};
      observableConfiguration.PropertyChanged += (sender, args) => { propertyName = args.PropertyName; };
      //Act
      observableConfiguration.SearchDesktopFilesDirectories.Add(new   DesktopFileDirectory(string.Empty, null));
      //Assert
      Assert.EndsWith(propertyName, $"[{observableConfiguration.SearchDesktopFilesDirectories.Count - 1}]");
   }
}