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
        private double pathPosition = 0;
        private int pathIndex = 0;
        private double speed;
        private List<PathPoint> path = new List<PathPoint>();
        private int fg_color;
        private int bg_color;

        public Planet(char symbol, int radius, double speed, int fg_color, int bg_color)
        {
            this.symbol = symbol;
            this.radius = radius;
            this.speed = speed;
            this.fg_color = fg_color;
            this.bg_color = bg_color;
        }

        public void DrawPathOnce()
        {
            Console.Write($"\x1b[38;5;{bg_color}m");
            foreach (PathPoint point in path)
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write(point.Symbol);
            }
            Console.Write("\x1b[0m");
        }

        public static List<(int X, int Y)> GenerateEllipse(int xc, int yc, int rx, int ry)
        {
            var uniquePoints = new HashSet<(int X, int Y)>();

            long rx2 = (long)rx * rx;
            long ry2 = (long)ry * ry;
            long twoRx2 = 2 * rx2;
            long twoRy2 = 2 * ry2;

            long x = 0;
            long y = ry;
            long prevX = 0;
            long prevY = ry;

            long p1 = (long)Math.Round(ry2 - rx2 * ry + 0.25 * rx2);
            long dx = 0;
            long dy = twoRx2 * y;

            while (dx < dy)
            {
                AddSymmetricPoints(uniquePoints, xc, yc, (int)x, (int)y);
                FillCornerGaps(uniquePoints, xc, yc, (int)prevX, (int)prevY, (int)x, (int)y);

                prevX = x;
                prevY = y;
                x++;
                dx = twoRy2 * x;

                if (p1 < 0)
                {
                    p1 += dx + ry2;
                }
                else
                {
                    y--;
                    dy = twoRx2 * y;
                    p1 += dx - dy + ry2;
                }
            }

            long p2 = (long)Math.Round(ry2 * (x + 0.5) * (x + 0.5) + rx2 * (y - 1) * (y - 1) - rx2 * ry2);

            while (y >= 0)
            {
                AddSymmetricPoints(uniquePoints, xc, yc, (int)x, (int)y);
                FillCornerGaps(uniquePoints, xc, yc, (int)prevX, (int)prevY, (int)x, (int)y);

                prevX = x;
                prevY = y;
                y--;
                dy = twoRx2 * y;

                if (p2 > 0)
                {
                    p2 += rx2 - dy;
                }
                else
                {
                    x++;
                    dx = twoRy2 * x;
                    p2 += dx - dy + rx2;
                }
            }

            var sortedPoints = uniquePoints.OrderBy(p =>
            {
                double angle = Math.Atan2(p.Y - yc, p.X - xc);
                if (angle < 0)
                {
                    angle += 2 * Math.PI;
                }
                return angle;
            }).ToList();

            return sortedPoints;
        }

        private static void AddSymmetricPoints(HashSet<(int X, int Y)> points, int xc, int yc, int x, int y)
        {
            points.Add((xc + x, yc + y));
            points.Add((xc - x, yc + y));
            points.Add((xc + x, yc - y));
            points.Add((xc - x, yc - y));
        }

        private static void FillCornerGaps(HashSet<(int X, int Y)> points, int xc, int yc, int prevX, int prevY, int currX, int currY)
        {
            if (Math.Abs(currX - prevX) > 0 && Math.Abs(currY - prevY) > 0)
            {
                // Quadrant 1 (+x, +y)
                points.Add((xc + currX, yc + prevY));
                // Quadrant 2 (-x, +y)
                points.Add((xc - currX, yc + prevY));
                // Quadrant 3 (+x, -y)
                points.Add((xc + currX, yc - prevY));
                // Quadrant 4 (-x, -y)
                points.Add((xc - currX, yc - prevY));
            }
        }

        public void GeneratePath()
        {
            // Generate coords
            var points = GenerateEllipse(Console.BufferWidth / 2, Console.BufferHeight / 2, (int)Math.Round(this.radius * 2.5), (int)Math.Round(this.radius * 0.8));

            foreach ((int X, int Y) point in points)
            {
                this.path.Add(new PathPoint(point.X, point.Y));
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
                    (-1, 0, 0, 1) => '╮',   // prev is LEFT, next is DOWN → lines go LEFT and DOWN
                    (-1, 0, 0, -1) => '╯',  // prev is LEFT, next is UP → lines go LEFT and UP
                    (1, 0, 0, 1) => '╭',    // prev is RIGHT, next is DOWN → lines go RIGHT and DOWN
                    (1, 0, 0, -1) => '╰',   // prev is RIGHT, next is UP → lines go RIGHT and UP

                    (0, -1, -1, 0) => '╯',  // prev is UP, next is LEFT → lines go UP and LEFT
                    (0, -1, 1, 0) => '╰',   // prev is UP, next is RIGHT → lines go UP and RIGHT
                    (0, 1, -1, 0) => '╮',   // prev is DOWN, next is LEFT → lines go DOWN and LEFT
                    (0, 1, 1, 0) => '╭',    // prev is DOWN, next is RIGHT → lines go DOWN and RIGHT

                    // Straight lines
                    (-1, 0, 1, 0) or (1, 0, -1, 0) => '─',  // Horizontal
                    (0, -1, 0, 1) or (0, 1, 0, -1) => '│',  // Vertical

                    _ => '?'
                };


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
                    Console.Write($"\x1b[38;5;{bg_color}m{pathPoint.Symbol}\x1b[0m");
                }
            }

            pathPosition += speed;
            pathIndex = ((int)pathPosition) % path.Count();

            var currentPoint = path[pathIndex];
            prevX = currentPoint.X;
            prevY = currentPoint.Y;

            Console.SetCursorPosition(currentPoint.X, currentPoint.Y);
            Console.Write($"\x1b[38;5;{fg_color}mO\x1b[0m");
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
        var planets = new Planet[] { new Planet('m', 4, 0.8, 15, 7), new Planet('v', 6, 0.575, 13, 5), new Planet('z', 8, 0.5, 10, 2), new Planet('m', 10, 0.4, 1, 9), new Planet('j', 13, 0.22, 11, 3), new Planet('s', 17, 0.1625, 15, 7), new Planet('u', 20, 0.145, 14, 6), new Planet('n', 23, 0.0925, 12, 4) };

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
