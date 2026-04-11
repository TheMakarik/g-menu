namespace GMenu.Modules;

public static class Guard
{
    public static void EnsureMemberNotNull<T>(T property, string memberName)
    {
        if (property is null)
            throw new InvalidOperationException($"Member {memberName} cannot be null.");
    }
}