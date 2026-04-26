namespace GMenu.Modules;

public static class Guard
{
    public static void EnsureMemberNotNull<T>(T member, string memberName)
    {
        if (member is null)
            throw new InvalidOperationException($"Member {memberName} cannot be null.");
    }
}