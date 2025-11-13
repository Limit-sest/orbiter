namespace orbiter;

class Program
{
    class Sun
    {
        public void ProcessTick()
        {
            Console.SetCursorPosition(Console.BufferWidth / 2, Console.BufferHeight / 2);
            Console.Write('@');
        }
    }

    class Planet
    {
        class PathPoint
        {
            public int X;
            public int Y;
            public char? Symbol;

            public PathPoint(int x, int y, char? symbol = ' ')
            {
                X = x;
                Y = y;
                Symbol = symbol;
            }
        }

        private char symbol;
        private int x;
        private int y;
        private int prevX = -1;
        private int prevY = -1;
        private int radius;
        private double angle = 0;
        private double speed;
        private List<PathPoint> path = new List<PathPoint>();

        public Planet(char symbol, int radius, double speed)
        {
            this.symbol = symbol;
            this.radius = radius;
            this.speed = speed;
        }

        public void DrawPathOnce()
        {
            foreach (PathPoint point in path)
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write(point.Symbol);
            }
        }

        public void GeneratePath()
        {
            // Generate coords
            for (double i = 0; i < 360; i += 0.01)
            {
                (int x, int y) = GetPos(i, this.radius);
                // Skip if same coords
                if (path.Count() > 0)
                {
                    var previousPoint = path.Last();
                    if ((x, y) == (previousPoint.X, previousPoint.Y)) continue;
                }
                path.Add(new PathPoint(x, y));
            }
            // Assign symbols
            for (int i = 0; i < path.Count(); i++)
            {
                var currentPoint = path[i];
                PathPoint previousPoint;
                PathPoint nextPoint;
                if (i - 1 < 0)
                {
                    previousPoint = path.Last();
                }
                else
                {
                    previousPoint = path[i - 1];
                }

                if (i + 1 < path.Count())
                {
                    nextPoint = path[i + 1];
                }
                else
                {
                    nextPoint = path[0];
                }

                currentPoint.Symbol = (previousPoint.X.CompareTo(currentPoint.X), previousPoint.Y.CompareTo(currentPoint.Y),
                                       nextPoint.X.CompareTo(currentPoint.X), nextPoint.Y.CompareTo(currentPoint.Y)) switch
                {
                    // Corner cases: (where is previous, where is next)
                    (-1, 0, 0, 1) => '┐',   // prev is LEFT, next is DOWN → lines go LEFT and DOWN
                    (-1, 0, 0, -1) => '┘',  // prev is LEFT, next is UP → lines go LEFT and UP
                    (1, 0, 0, 1) => '┌',    // prev is RIGHT, next is DOWN → lines go RIGHT and DOWN
                    (1, 0, 0, -1) => '└',   // prev is RIGHT, next is UP → lines go RIGHT and UP

                    (0, -1, -1, 0) => '┘',  // prev is UP, next is LEFT → lines go UP and LEFT
                    (0, -1, 1, 0) => '└',   // prev is UP, next is RIGHT → lines go UP and RIGHT
                    (0, 1, -1, 0) => '┐',   // prev is DOWN, next is LEFT → lines go DOWN and LEFT
                    (0, 1, 1, 0) => '┌',    // prev is DOWN, next is RIGHT → lines go DOWN and RIGHT

                    // Straight lines
                    (-1, 0, 1, 0) or (1, 0, -1, 0) => '─',  // Horizontal
                    (0, -1, 0, 1) or (0, 1, 0, -1) => '│',  // Vertical

                    _ => '?'
                };
                if (currentPoint.Symbol)


            }
        }


        private (int x, int y) GetPos(double angle, int radius)
        {
            x = Console.BufferWidth / 2 + (int)Math.Round(Math.Cos(angle) * radius * 2.5);
            y = Console.BufferHeight / 2 + (int)Math.Round(Math.Sin(angle) * radius * 0.8);
            return (x, y);
        }

        public void ProcessTick()
        {
            if (prevX >= 0 && prevY >= 0)
            {
                var pathPoint = path.FirstOrDefault(p => p.X == prevX && p.Y == prevY);
                if (pathPoint != null)
                {
                    Console.SetCursorPosition(prevX, prevY);
                    Console.Write(pathPoint.Symbol);
                }
            }

            (this.x, this.y) = GetPos(this.angle, this.radius);
            angle -= speed;
            if (angle <= 0)
            {
                angle = 360;
            }
            Console.SetCursorPosition(this.x, this.y);
            Console.Write(symbol);
            prevX = this.x;
            prevY = this.y;
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
        var planets = new Planet[] { new Planet('m', 5, 0.032), new Planet('v', 7, 0.023), new Planet('z', 10, 0.02), new Planet('m', 13, 0.016), new Planet('j', 15, 0.0088), new Planet('s', 17, 0.0065), new Planet('u', 19, 0.0058), new Planet('n', 21, 0.0037) };

        // Start asynchronous keypress handler
        var keyTask = Task.Run(() => HandleKeyPress());

        Console.Clear();
        foreach (Planet planet in planets)
        {
            planet.GeneratePath();
            planet.DrawPathOnce();
        }
        sun.ProcessTick();
        DrawControls();

        try
        {
            while (_running)
            {
                foreach (Planet planet in planets)
                {
                    planet.ProcessTick();
                }
                await Task.Delay(500);
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
