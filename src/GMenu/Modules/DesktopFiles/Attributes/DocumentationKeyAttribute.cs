namespace GMenu.Modules.DesktopFiles.Attributes;

[AttributeUsage( AttributeTargets.Property)]
public sealed class DocumentationKeyAttribute(string key) : Attribute
{
    public string Key { get; } = key;
}