namespace orbiter.Helpers;

public static class ConsoleHelper
{
    public static void SafeSetCursorPosition(int x, int y)
    {
        int safeX = Math.Max(0, Math.Min(x, Console.BufferWidth - 1));
        int safeY = Math.Max(0, Math.Min(y, Console.BufferHeight - 1));
        Console.SetCursorPosition(safeX, safeY);
    }
}
