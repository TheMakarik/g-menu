namespace GMenu.Modules.DesktopFiles;

public sealed class DesktopFileRemover :  IDesktopFileRemover
{
    public ValueTask RemoveAsync(string filePath)
    {
          Debug.Assert(File.Exists(filePath));
          try
          {
                File.Delete(filePath);
          }
          catch (UnauthorizedAccessException)
          {
              
          }
          
          return ValueTask.CompletedTask;
    }
}