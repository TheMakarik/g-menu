using GMenu.Modules.ColorUtils.Model;
using GMenu.Modules.Configuration.Model;

namespace GMenu.Testing;

public class ObservableConfigurationTests
{
   
   [Fact]
   public void AllProperties_WithNotNullValues_MustRaiseINotifyPropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() 
      { 
          UnexistingCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true; 
      };
      
      observableConfiguration.BeginPropertyChangeRaising();
      
      // Act
      observableConfiguration.UnexistingCategories = [new UnexistingCategory(){Path = string.Empty, Name = string.Empty}];
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void ObservableCollection_OnValueChanging_MustRaiseINotifyPropertyChangedWithIndex()
   {
      // Arrange
      var propertyName = string.Empty;
      var observableConfiguration = new ObservableConfiguration
      {
          UnexistingCategories =
          [
          ],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          propertyName = args.PropertyName; 
      };
      
      observableConfiguration.BeginPropertyChangeRaising();
      
      // Act
      observableConfiguration.UnexistingCategories.Add(new UnexistingCategory(){Path = string.Empty, Name = string.Empty});
      
      // Assert
      Assert.EndsWith($"[{observableConfiguration.UnexistingCategories.Count - 1}]", propertyName);
   }
   
   [Fact]
   public void ObservableCollection_OnCollectionReset_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() 
      { 
          UnexistingCategories = [new UnexistingCategory(){Path = string.Empty, Name = string.Empty}, new UnexistingCategory(){Path = string.Empty, Name = string.Empty}],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true;
      };
      
      observableConfiguration.BeginPropertyChangeRaising();
      
      // Act
      observableConfiguration.UnexistingCategories.Clear();
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void LocalizeDesktopFileNames_WhenChanged_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() 
      { 
          UnexistingCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          LocalizeDesktopFiles = false,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          if (args.PropertyName == nameof(ObservableConfiguration.LocalizeDesktopFiles))
              wasPropertyChangedCalled = true;
      };
      
      observableConfiguration.BeginPropertyChangeRaising();
      
      // Act
      observableConfiguration.LocalizeDesktopFiles = true;
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void Language_WhenChanged_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() 
      { 
          UnexistingCategories = [],
          Language = null!,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          if (args.PropertyName == nameof(ObservableConfiguration.Language))
              wasPropertyChangedCalled = true;
      };
      
      observableConfiguration.BeginPropertyChangeRaising();
      
      // Act
      observableConfiguration.Language = new System.Globalization.CultureInfo("ru-RU");
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void BeginPropertyChangeRaising_WhenNotCalled_ShouldNotRaiseEvents()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var observableConfiguration = new ObservableConfiguration() 
      { 
          UnexistingCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          Version = null!
      };
      
      observableConfiguration.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true; 
      };
      
      // Act
      observableConfiguration.Language = new System.Globalization.CultureInfo("ru-RU");
      
      // Assert
      Assert.False(wasPropertyChangedCalled);
   }
}