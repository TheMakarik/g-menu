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
      var systemUnderTests = new ObservableConfiguration() 
      { 
          CustomCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true; 
      };
      
      systemUnderTests.BeginPropertyChangeRaising();
      
      // Act
      systemUnderTests.CustomCategories = [new CustomUserCategory(){Name = string.Empty, LocalizedName = string.Empty}];
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void ObservableCollection_OnValueChanging_MustRaiseINotifyPropertyChangedWithIndex()
   {
      // Arrange
      var propertyName = string.Empty;
      var systemUnderTests = new ObservableConfiguration
      {
          CustomCategories =
          [
          ],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          propertyName = args.PropertyName; 
      };
      
      systemUnderTests.BeginPropertyChangeRaising();
      
      // Act
      systemUnderTests.CustomCategories.Add(new CustomUserCategory(){Name = string.Empty, LocalizedName = string.Empty});
      
      // Assert
      Assert.EndsWith($"[{systemUnderTests.CustomCategories.Count - 1}]", propertyName);
   }
   
   [Fact]
   public void ObservableCollection_OnCollectionReset_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var systemUnderTests = new ObservableConfiguration() 
      { 
          CustomCategories = [new CustomUserCategory(){Name = string.Empty, LocalizedName = string.Empty}, new CustomUserCategory(){Name = string.Empty, LocalizedName = string.Empty}],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          AccentColor = "#FFFFFF",
          Theme = BaseTheme.Light,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true;
      };
      
      systemUnderTests.BeginPropertyChangeRaising();
      
      // Act
      systemUnderTests.CustomCategories.Clear();
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void LocalizeDesktopFileNames_WhenChanged_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var systemUnderTests = new ObservableConfiguration() 
      { 
          CustomCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          LocalizeDesktopFiles = false,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          if (args.PropertyName == nameof(ObservableConfiguration.LocalizeDesktopFiles))
              wasPropertyChangedCalled = true;
      };
      
      systemUnderTests.BeginPropertyChangeRaising();
      
      // Act
      systemUnderTests.LocalizeDesktopFiles = true;
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void Language_WhenChanged_MustRaisePropertyChanged()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var systemUnderTests = new ObservableConfiguration() 
      { 
          CustomCategories = [],
          Language = null!,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          if (args.PropertyName == nameof(ObservableConfiguration.Language))
              wasPropertyChangedCalled = true;
      };
      
      systemUnderTests.BeginPropertyChangeRaising();
      
      // Act
      systemUnderTests.Language = new System.Globalization.CultureInfo("ru-RU");
      
      // Assert
      Assert.True(wasPropertyChangedCalled);
   }
   
   [Fact]
   public void BeginPropertyChangeRaising_WhenNotCalled_ShouldNotRaiseEvents()
   {
      // Arrange
      var wasPropertyChangedCalled = false;
      var systemUnderTests = new ObservableConfiguration() 
      { 
          CustomCategories = [],
          Language = System.Globalization.CultureInfo.CurrentCulture,
          Version = null!
      };
      
      systemUnderTests.PropertyChanged += (sender, args) => 
      { 
          wasPropertyChangedCalled = true; 
      };
      
      // Act
      systemUnderTests.Language = new System.Globalization.CultureInfo("ru-RU");
      
      // Assert
      Assert.False(wasPropertyChangedCalled);
   }
}