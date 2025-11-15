namespace orbiter;

class Program
{
    public static Models.Planet[] planets = new Models.Planet[] {
        new Models.Planet("M", "Mercury", 4, 0.8, 15, 7),
        new Models.Planet("V", "Venus", 6, 0.575, 13, 5),
        new Models.Planet("E", "Earth", 8, 0.5, 10, 2, new List<Models.Planet.Moon> { new Models.Planet.Moon(1, 0.1, 10) }),
        new Models.Planet("M", "Mars", 10, 0.4, 1, 9),
        new Models.Planet("J", "Jupiter", 13, 0.22, 11, 3),
        new Models.Planet("S", "Saturn", 17, 0.1625, 15, 7),
        new Models.Planet("U", "Uranus", 20, 0.145, 14, 6),
        new Models.Planet("N", "Neptune", 23, 0.0925, 12, 4) };

    static void DrawControls()
    {
        Console.SetCursorPosition(1, Console.BufferHeight);
        Console.Write("\x1b[7m q \x1b[27m quit   \x1b[7m ← \x1b[27m speed \x1b[7m → \x1b[27m   \x1b[7m space \x1b[27m toggle labels");
    }

    static async Task Main(string[] args)
    {
        // Console setup
        Console.Write("\x1b[?1049h\x1b[H"); // Alternate buffer
        Console.CursorVisible = false;

        var sun = new Models.Sun();

        // Start asynchronous keypress handler
        var keyTask = Task.Run(() => HandleKeyPress());

        Console.Clear();
        for (int i = 0; i < planets.Length; i++)
        {
            planets[i].GeneratePath();
            planets[i].DrawPathOnce();
            planets[i].DrawLabel(i, !AppState.LabelsShown);
        }
        sun.ProcessTick();
        DrawControls();

        try
        {
            while (AppState.Running)
            {
                foreach (Models.Planet planet in planets)
                {
                    planet.ProcessTick();
                }
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
        while (AppState.Running)
        {
            if (Console.KeyAvailable)
            {
                var key = Console.ReadKey(true);

                switch (key.Key)
                {
                    case ConsoleKey.Q:
                        AppState.Running = false;
                        break;
                    case ConsoleKey.Escape:
                        AppState.Running = false;
                        break;
                    case ConsoleKey.RightArrow:
                        AppState.SpeedMultiplier += 0.1;
                        break;
                    case ConsoleKey.LeftArrow:
                        if (AppState.SpeedMultiplier > 0.1) AppState.SpeedMultiplier -= 0.1;
                        break;
                    case ConsoleKey.Spacebar:
                        AppState.LabelsShown = !AppState.LabelsShown;
                        for (int i = 0; i < planets.Length; i++)
                        {
                            planets[i].DrawLabel(i, !AppState.LabelsShown);
                        }
                        break;
                }

            }
            Thread.Sleep(25);
        }
    }
}
