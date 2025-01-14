namespace PlainDeck.Extensions;

public static partial class KeyPress
{
    public static void KeyDown(ConsoleKey key)
    {
        if (OperatingSystem.IsWindows())
        {
            KeyDown_Windows(key);

            return;
        }
        
        if (OperatingSystem.IsMacOS())
        {
            KeyDown_MacOS(key);

            return;
        }
        
        throw new NotSupportedException("Platform not supported");
    }
    
    public static void KeyUp(ConsoleKey key)
    {
        if (OperatingSystem.IsWindows())
        {
            KeyUp_Windows(key);

            return;
        }
        
        if (OperatingSystem.IsMacOS())
        {
            KeyUp_MacOS(key);

            return;
        }

        throw new NotSupportedException("Platform not supported");
    }
}
