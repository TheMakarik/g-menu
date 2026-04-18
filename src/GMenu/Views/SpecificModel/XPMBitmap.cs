namespace GMenu.Views.SpecificModel;

public sealed class XPMBitmap(string filePath)
{
    public const string X11 = "libX11.so";

    [DllImport(X11)]
    private static extern IntPtr XOpenDisplay(string? display_name);
    
    [DllImport(X11)]
    private static extern int XCloseDisplay(IntPtr display);
    
    [StructLayout(LayoutKind.Sequential)]
    public struct XImage
    {
        public int width;
        public int height;
        public int xoffset;              
        public int format;            

        public IntPtr data;              

        public int byte_order;           
        public int bitmap_unit;         
        public int bitmap_bit_order;  
        public int bitmap_pad;        
        public int depth;                

        public int bytes_per_line;      
        public int bits_per_pixel;       

        public ulong red_mask;          
        public ulong green_mask;         
        public ulong blue_mask;          
        
    }
    
}