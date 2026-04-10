using System.Diagnostics.CodeAnalysis;
using GMenu.Modules.Localization.Model;

namespace GMenu.Modules.DesktopFiles.Model;

public record struct DesktopFileDirectory(string Path, LocalizationKey? Name);