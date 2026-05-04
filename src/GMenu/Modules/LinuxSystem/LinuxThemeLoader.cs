namespace GMenu.Modules.LinuxSystem;

public sealed class LinuxThemeLoader(ILogger logger) : ILinuxThemeLoader
{
    public async Task<Rgb?> GetThemeHexAsync()
    {
        // https://stackoverflow.com/questions/79934943/tmds-dbus-query-works-in-the-consoleapp-but-thrown-exception-in-the-my-avaloniau
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

            var (red, green, blue) = await connection.CallMethodAsync(message, (m, _) =>
            {                                                                                                                 
                var reader = m.GetBodyReader();
                reader.AlignStruct();                                                                                         
                reader.ReadInt64();
                return ((int)(reader.ReadDouble() * 255),
                    (int)(reader.ReadDouble() * 255),
                    (int)(reader.ReadDouble() * 255));                                                                    
            }, null);

            logger.Information("Load colors: {red}, {green}, {blue}", red, green, blue);
            return new Rgb(red, green, blue);
        }
        catch (Exception e)
        {
            logger.Error("Exception: {exception}", e);
            return null;
        }
       
    }
}