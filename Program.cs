namespace orbiter;

class Program
{
    public static Models.Planet[] planets = new Models.Planet[] {
        new Models.Planet("M", "Mercury", 4, 0.8, 15, 7),
        new Models.Planet("V", "Venus", 6, 0.575, 13, 5),
        new Models.Planet("E", "Earth", 8, 0.5, 10, 2, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(2, 0.05, 10, '᳃')
        }),
        new Models.Planet("M", "Mars", 10, 0.4, 1, 9, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(0.7, 0.5, 1, '·'),
            new Models.Planet.Moon(1.5, 0.3, 1, '·')
        }),
        new Models.Planet("J", "Jupiter", 13, 0.22, 11, 3, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(0.8, 0.5, 11, '᳃'),
            new Models.Planet.Moon(1.5, 0.32, 11, '⸰'),
            new Models.Planet.Moon(1.8, 0.1, 11, '○'),
            new Models.Planet.Moon(2, 0.07, 11, '○')
        }),
        new Models.Planet("S", "Saturn", 17, 0.1625, 15, 7, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(0.8, 0.4, 15, '·'),
            new Models.Planet.Moon(1.5, 0.1, 15, '⸰'),
            new Models.Planet.Moon(1.9, 0.07, 15, '○'),
            new Models.Planet.Moon(2.2, 0.04, 15, '⸰')
        }),
        new Models.Planet("U", "Uranus", 20, 0.145, 14, 6, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(0.8, 0.32, 14, '⸰'),
            new Models.Planet.Moon(1.0, 0.3, 14, '⸰'),
            new Models.Planet.Moon(1.5, 0.1, 14, '⸰'),
            new Models.Planet.Moon(1.7, 0.08, 14, '⸰')
        }),
        new Models.Planet("N", "Neptune", 23, 0.0925, 12, 4, new List<Models.Planet.Moon> {
            new Models.Planet.Moon(1.0, -0.4, 12, '᳃')
        })
    };

    static void DrawControls()
    {
        Helpers.ConsoleHelper.SafeSetCursorPosition(1, Console.BufferHeight);
        Console.Write("\x1b[7m Q \x1b[27m quit   \x1b[7m ← \x1b[27m speed \x1b[7m → \x1b[27m   \x1b[7m space \x1b[27m pause   \x1b[7m L \x1b[27m toggle labels   \x1b[7m M \x1b[27m toggle moons");
    }

    static async Task Main(string[] args)
    {
        Models.Planet.ScalingFactor = ((Console.BufferWidth) * 0.90) / (23.0 * 2.5);
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
                        if (AppState.SpeedMultiplier == 0)
                        {
                            AppState.SpeedMultiplier = AppState.OriginalSpeed;
                        }
                        else
                        {
                            AppState.OriginalSpeed = AppState.SpeedMultiplier;
                            AppState.SpeedMultiplier = 0;
                        }
                        break;
                    case ConsoleKey.L:
                        AppState.LabelsShown = !AppState.LabelsShown;
                        for (int i = 0; i < planets.Length; i++)
                        {
                            planets[i].DrawLabel(i, !AppState.LabelsShown);
                        }
                        break;
                    case ConsoleKey.M:
                        AppState.MoonsShown = !AppState.MoonsShown;
                        foreach (var planet in planets)
                        {
                            if (AppState.MoonsShown) planet.DrawMoons();
                            else planet.EraseMoons();
                        }
                        break;
                }

            }
            Thread.Sleep(25);
        }
    }
}
