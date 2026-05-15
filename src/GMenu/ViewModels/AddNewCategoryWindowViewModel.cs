using System.Linq.Expressions;

namespace GMenu.ViewModels;

public sealed partial class AddNewCategoryWindowViewModel : ViewModelBase
{
    [Reactive] private string? _unlocalizedCategory;
    [Reactive] private string? _localizedCategory = string.Empty;
    [Reactive] private string _iconKind = string.Empty;
    [Reactive] private string? _searchIconText = null;
    [Reactive] private ObservableCollection<string> _icons = [];

    public Interaction<string, IEnumerable<string>> GetIconsInteraction { get;  }= new();

    private ICollection<string>? _allCategories;
    private bool _wasLoadedOnce = false;
    private readonly ILogger _logger;
    private ICollection<string>? _iconsKindCollection;
    private readonly ICustomUserCategoryManager _categoryManager;

    public AddNewCategoryWindowViewModel(ILocalizationProvider localizationProvider, ILogger logger, ICustomUserCategoryManager categoryManager) : base(
        localizationProvider)
    {
        _logger = logger;
        _categoryManager = categoryManager;
      
        MessageBus.Current.SendMessage(new GetCategoriesListMessage()
        {
            SetCategoriesListAction = result =>
            {
                _allCategories = result;
                logger.Debug("All Categories: {categories}", string.Join(", ", _allCategories));
                RestartValidators();
            },
        });

#pragma warning disable IL2026

        this.ValidationRule(
            static viewModel => viewModel.LocalizedCategory,
            (value) => !string.IsNullOrWhiteSpace(value),
            (_) => this.LocalizationProvider["ValueMustBeNotEmpty"]);

        this.ValidationRule(
            static viewModel => viewModel.UnlocalizedCategory,
            (value) => !string.IsNullOrWhiteSpace(value),
            (_) => this.LocalizationProvider["ValueMustBeNotEmpty"]);

        this.ValidationRule(
            static viewModel => viewModel.UnlocalizedCategory,
            (value) => !(_allCategories ?? []).Contains(value)
                       && !(_allCategories ?? []).Contains("X-" + value ?? string.Empty),
            (_) => this.LocalizationProvider["CategoryAlreadyExists"]);

        this.ValidationRule(
            static viewModel => viewModel.SearchIconText,
            (value) => value.ContainsOnlyLatinAndNumbersAndUnderline(),
            _ => this.LocalizationProvider["StringMustContainOnlyLatinAndNumbersAndSpecialSymbols"]
        );

        this.ValidationRule(
            static viewModel => viewModel.UnlocalizedCategory,
            (value) => value.ContainsOnlyLatinAndNumbersAndUnderline(),
            _ => this.LocalizationProvider["StringMustContainOnlyLatinAndNumbersAndSpecialSymbols"]
        );


#pragma warning restore IL2026
        
    }

 

    [ReactiveCommand]
    private void SetIconKind(string value)
    {
        IconKind = value;
        _logger.Debug("Selected icon for category {unlocalized}-{localized}: {icon}", UnlocalizedCategory,
            LocalizedCategory, value);
    }

    [ReactiveCommand]
    private void LoadIconsKind()
    {
        if (_wasLoadedOnce)
            return;
        _wasLoadedOnce = true;

        this.WhenPropertyChanged(viewModel => viewModel.SearchIconText)
            .Subscribe(async void (args) =>
            {
                var icons = await GetIconsInteraction.Handle(args.Value ?? string.Empty);
                var gotAllIcons = WasGetAllIcons(args.Value);
                if (gotAllIcons)
                    _iconsKindCollection ??= [];
                Icons.Clear();
                foreach (var icon in icons)
                {
                    if(gotAllIcons && !(_iconsKindCollection?.Any() ?? false))
                        _iconsKindCollection?.Add(icon);
                         
                    RxSchedulers.MainThreadScheduler.Schedule(() => { Icons.Add(icon); });
                }
            });

        this.RaisePropertyChanged(nameof(SearchIconText)); //To start loading icons
    }
    
    
    [ReactiveCommand]
    private void ClearSearchTextAndIcons()
    {
        SearchIconText = null;
        Icons.Clear();
    }

    [ReactiveCommand(CanExecute = nameof(HasErrors))]
    private void CreateNewCategory()
    {
        var newCategory = new CustomUserCategory()
        {
            LocalizedName = LocalizedCategory!,
            Name = "X-" + UnlocalizedCategory!,
            IconKind = IconKind
        };
         _categoryManager.Add(newCategory);
         _allCategories?.Add(newCategory.Name);
         RestartValidators();
         MessageBus.Current.SendMessage(new AddCategoryMessage(){Category = new CategoryTreeViewInfo()
         {
             Headers = null, 
             Icon = newCategory.IconKind,
             Name = newCategory.LocalizedName 
         }});
    }

    private static bool WasGetAllIcons(string? searchText)
    {
        return string.IsNullOrWhiteSpace(searchText);
    }
    
    private void RestartValidators()
    {
        this.RaisePropertyChanged(); //To restart validators
    }


}