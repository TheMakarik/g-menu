namespace GMenu.Modules.LinuxSystem;

public sealed class LinuxThemeLoader(ILogger logger) : ILinuxThemeLoader
{
    public const string AccentColorNamespace = "org.freedesktop.appereance";
    public const string AccentColorName = "accent-color";
    
    
    public async Task<Rgb?> GetThemeHexAsync()
    {
        try
        {
            logger.Debug("Getting theme from {namespace} {name}", AccentColorNamespace, AccentColorName);
            var writer = DBusConnection.Session.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: "org.freedesktop.portal.Desktop",
                path: "/org/freedesktop/portal/desktop",
                @interface: "org.freedesktop.portal.Settings", 
                member: "ReadOne",
                signature: "ss");
        
            writer.WriteString(AccentColorNamespace);
            writer.WriteString(AccentColorName);

            var message = writer.CreateMessage();

            var reply = await DBusConnection.Session.CallMethodAsync(message, 
                (result, _) => result);

            var reader = reply.GetBodyReader();
            reader.AlignStruct();   
            
            reader.ReadInt64(); //First value is strange, IDK why but I send the issue: 
            var red = (byte)(reader.ReadDouble() * 255);
            var green = (byte)(reader.ReadDouble() * 255);
            var blue = (byte)(reader.ReadDouble() * 255);
            
            logger.Information("Found RGB: {r}, {g}, {b}", red, green, blue);
            return new Rgb(red, green, blue);   

        }
        catch (Exception e)
        {
            logger.Error("Exception: {exception}", e);
            return null;
        }
       
    }
}