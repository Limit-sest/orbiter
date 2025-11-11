namespace orbiter;

class Program
{
    static void Main(string[] args)
    {
        // Console setup
        Console.Write("\x1b[?1049h\x1b[H"); // Alternate buffer
        Console.CursorVisible = false;

        Console.SetCursorPosition(Console.BufferWidth / 2, Console.BufferHeight / 2);
        Console.Write('@');
        Thread.Sleep(2200);
        Console.Write("\x1b[?1049l"); // Main buffer
    }
}
