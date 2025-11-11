namespace orbiter;

class Program
{
    class Sun()
    {
        public void ProcessTick()
        {
            Console.SetCursorPosition(Console.BufferWidth / 2, Console.BufferHeight / 2);
            Console.Write('@');
        }
    }

    private static volatile bool _running = true;

    static void DrawControls()
    {
        Console.SetCursorPosition(1, Console.BufferHeight);
        Console.Write("\x1b[7m q \x1b[27m quit");
    }

    static async Task Main(string[] args)
    {
        // Console setup
        Console.Write("\x1b[?1049h\x1b[H"); // Alternate buffer
        Console.CursorVisible = false;

        var sun = new Sun();

        // Start asynchronous keypress handler
        var keyTask = Task.Run(() => HandleKeyPress());

        try
        {
            while (_running)
            {
                Console.Clear();
                sun.ProcessTick();
                DrawControls();
                await Task.Delay(200);
            }
        }
        finally
        {
            Console.Write("\x1b[?1049l"); // Return to main buffer
            Console.CursorVisible = true;
        }

        await keyTask;
    }

    private static void HandleKeyPress()
    {
        while (_running)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                // Exit on 'q' or 'Esc'
                if (key.Key == ConsoleKey.Q || key.Key == ConsoleKey.Escape)
                {
                    _running = false;
                    break;
                }
            }
            Thread.Sleep(50);
        }
    }
}
