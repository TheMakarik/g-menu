
namespace GMenu.Modules.Configuration.Model;

[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties)]
public sealed class CustomUserCategory
{
    public required string Name { get; set; }
    public required string LocalizedName { get; set; }
    public string? IconKind { get; set; }
}