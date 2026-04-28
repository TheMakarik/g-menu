namespace GMenu.Modules.LinuxSystem;

public sealed class LinuxThemeLoader(ILogger logger) : ILinuxThemeLoader
{
    public async Task<Rgb?> GetThemeHexAsync()
    {
        try
        {
            var connection = new DBusConnection(DBusAddress.Session!);
            await connection.ConnectAsync();
            var writer = connection.GetMessageWriter();
            writer.WriteMethodCallHeader(
                destination: "org.freedesktop.portal.Desktop",
                path: "/org/freedesktop/portal/desktop",
                @interface: "org.freedesktop.portal.Settings", 
                member: "ReadOne",
                signature: "ss");
        
            writer.WriteString("org.freedesktop.appearance");
            writer.WriteString("accent-color");

            var message = writer.CreateMessage();

            var reply = await connection.CallMethodAsync(message, 
                (result, _) => result);

            var reader = reply.GetBodyReader();
            reader.AlignStruct();   
            
            reader.ReadInt64();
            var red = (int)(reader.ReadDouble() * 255);
            var green = (int)(reader.ReadDouble() * 255);
            var blue = (int)(reader.ReadDouble() * 255);

            return new Rgb(red, green, blue);
        }
        catch (Exception e)
        {
            logger.Error("Exception: {exception}", e);
            return null;
        }
       
    }
}