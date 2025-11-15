namespace orbiter.Models;

public class Planet
{
    class PathPoint
    {
        public int X;
        public int Y;
        public string? Symbol;
        public int Color;

        public PathPoint(int x, int y, string? symbol = " ", int color = 0)
        {
            X = x;
            Y = y;
            Symbol = symbol;
            Color = color;
        }
    }

    public class Moon
    {
        private char symbol;
        private int prevX = -1;
        private int prevY = -1;
        private int x = -1;
        private int y = -1;
        private double radius;
        private double angle = 0;
        private double speed;
        public int Color;
        public string Name;

        public Moon(double radius, double speed, int color, string name, char symbol = '◦')
        {
            this.radius = radius;
            this.speed = speed;
            this.Color = color;
            this.Name = name;
            this.symbol = symbol;
        }

        private void SetPosition(int planetX, int planetY)
        {
            this.x = planetX + (int)Math.Round(Math.Cos(this.angle) * this.radius * 2.25);
            this.y = planetY + (int)Math.Round(Math.Sin(this.angle) * this.radius);

        }

        public void Draw()
        {
            if (x >= 0 && y >= 0)
            {
                Helpers.ConsoleHelper.SafeSetCursorPosition(x, y);
                Console.Write($"\x1b[38;5;{Color}m{symbol}\x1b[0m");
            }
        }

        public void Erase(int planetX, int planetY)
        {
            if (x >= 0 && y >= 0 && !(x == planetX && y == planetY))
            {
                Helpers.ConsoleHelper.SafeSetCursorPosition(x, y);
                var key = (x, y);
                if (paths.ContainsKey(key))
                {
                    var restoringPoint = paths[key].First();
                    Console.Write($"\x1b[38;5;{restoringPoint.Color}m{restoringPoint.Symbol}\x1b[0m");
                }
                else
                {
                    Console.Write(' ');
                }
            }
        }

        public void ProcessTick(int planetX, int planetY)
        {
            SetPosition(planetX, planetY);

            if ((prevX >= 0 && prevY >= 0) && (prevX != x || prevY != y))
            {
                var key = (prevX, prevY);
                Helpers.ConsoleHelper.SafeSetCursorPosition(prevX, prevY);
                if (paths.ContainsKey(key))
                {
                    var restoringPoint = paths[key].First();
                    Console.Write($"\x1b[38;5;{restoringPoint.Color}m{restoringPoint.Symbol}\x1b[0m");
                }
                else if (!(prevX == planetX && prevY == planetY))
                {
                    Console.Write(' ');
                }
            }

            if (AppState.MoonsShown)
            {
                if (prevX != x || prevY != y)
                {
                    Helpers.ConsoleHelper.SafeSetCursorPosition(this.x, this.y);
                    Console.Write($"\x1b[38;5;{Color}m{symbol}\x1b[0m");
                }

            }
            angle += speed * AppState.SpeedMultiplier;

            if (angle > 360) angle -= 360;
            if (angle < 0) angle += 360;

            this.prevX = this.x;
            this.prevY = this.y;
        }
    }

    private static Dictionary<(int X, int Y), List<PathPoint>> paths = new Dictionary<(int X, int Y), List<PathPoint>>();
    public static double ScalingFactor { get; set; } = 1.0;

    private string symbol;
    private string name;
    private int prevX = -1;
    private int prevY = -1;
    private double radius;
    private double pathPosition = 0;
    private int pathIndex = 0;
    private double speed;
    private int fg_color;
    private int bg_color;
    private List<PathPoint> path = new List<PathPoint>();
    private List<Moon>? moons;

    public Planet(string symbol, string name, int radius, double speed, int fg_color, int bg_color, List<Moon>? moons = null)
    {
        this.symbol = symbol;
        this.name = name;
        this.radius = radius * ScalingFactor;
        this.speed = speed;
        this.fg_color = fg_color;
        this.bg_color = bg_color;
        this.moons = moons;
    }

    public void DrawPathOnce()
    {
        Console.Write($"\x1b[38;5;{bg_color}m");
        foreach (PathPoint point in path)
        {
            Helpers.ConsoleHelper.SafeSetCursorPosition(point.X, point.Y);
            Console.Write(point.Symbol);
        }
        Console.Write("\x1b[0m");
    }

    public void DrawLabel(int offset, bool erase = false)
    {
        Helpers.ConsoleHelper.SafeSetCursorPosition(0, offset);
        if (erase)
        {
            Console.Write(new String(' ', this.name.Count()));
        }
        else
        {
            Console.Write($"\x1b[38;5;{bg_color}m{this.name}\x1b[0m");
        }
    }



    public void GeneratePath()
    {
        // Generate coords
        var points = Helpers.EllipseGenerator.GenerateEllipse(Console.BufferWidth / 2, Console.BufferHeight / 2, (int)Math.Round(this.radius), (int)Math.Round(this.radius * 0.32));
        foreach ((int X, int Y) point in points)
        {
            var pathPoint = new PathPoint(point.X, point.Y, color: bg_color);
            this.path.Add(pathPoint);
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
                (-1, 0, 0, 1) => "╮",   // prev is LEFT, next is DOWN → lines go LEFT and DOWN
                (-1, 0, 0, -1) => "╯",  // prev is LEFT, next is UP → lines go LEFT and UP
                (1, 0, 0, 1) => "╭",    // prev is RIGHT, next is DOWN → lines go RIGHT and DOWN
                (1, 0, 0, -1) => "╰",   // prev is RIGHT, next is UP → lines go RIGHT and UP

                (0, -1, -1, 0) => "╯",  // prev is UP, next is LEFT → lines go UP and LEFT
                (0, -1, 1, 0) => "╰",   // prev is UP, next is RIGHT → lines go UP and RIGHT
                (0, 1, -1, 0) => "╮",   // prev is DOWN, next is LEFT → lines go DOWN and LEFT
                (0, 1, 1, 0) => "╭",    // prev is DOWN, next is RIGHT → lines go DOWN and RIGHT

                // Straight lines
                (-1, 0, 1, 0) or (1, 0, -1, 0) => "─",  // Horizontal
                (0, -1, 0, 1) or (0, 1, 0, -1) => "│",  // Vertical

                _ => "?"
            };
        }

        foreach (var point in path)
        {
            var key = (point.X, point.Y);
            if (!paths.ContainsKey(key))
            {
                paths[key] = new List<PathPoint>();
            }
            paths[key].Add(point);
        }
    }


    public void ProcessTick()
    {
        if (prevX >= 0 && prevY >= 0)
        {
            var key = (prevX, prevY);
            if (paths.ContainsKey(key))
            {
                var restoringPoint = paths[key].First();
                Helpers.ConsoleHelper.SafeSetCursorPosition(prevX, prevY);
                Console.Write($"\x1b[38;5;{restoringPoint.Color}m{restoringPoint.Symbol}\x1b[0m");
            }
        }



        pathPosition += speed * AppState.SpeedMultiplier;
        pathIndex = ((int)pathPosition) % path.Count();

        var currentPoint = path[pathIndex];
        prevX = currentPoint.X;
        prevY = currentPoint.Y;

        if (moons != null)
        {
            foreach (Moon moon in moons)
            {
                moon.ProcessTick(currentPoint.X, currentPoint.Y);
            }
        }

        Helpers.ConsoleHelper.SafeSetCursorPosition(currentPoint.X, currentPoint.Y);
        if (AppState.LabelsShown)
        {
            Console.Write($"\x1b[38;5;{fg_color}m{symbol}\x1b[0m");
        }
        else
        {
            Console.Write($"\x1b[38;5;{fg_color}m⬤\x1b[0m");
        }
    }

    public void DrawMoons()
    {
        if (moons != null)
        {
            foreach (Moon moon in moons)
            {
                moon.Draw();
            }
        }
    }

    public void EraseMoons()
    {
        if (moons != null)
        {
            var currentPoint = path[pathIndex];
            foreach (Moon moon in moons)
            {
                moon.Erase(currentPoint.X, currentPoint.Y);
            }
        }
    }

    public bool HasMoons()
    {
        return moons != null && moons.Count > 0;
    }

    public int GetMoonCount()
    {
        return moons?.Count ?? 0;
    }

    public void DrawMoonLabels(int startRow, bool erase = false)
    {
        if (moons != null)
        {
            int row = startRow;
            for (int i = 0; i < moons.Count(); i++)
            {
                var moon = moons[i];
                Helpers.ConsoleHelper.SafeSetCursorPosition(0, row);
                if (erase)
                {
                    Console.Write(new String(' ', moon.Name.Length + 2));
                }
                else
                {
                    char line = '⎬';
                    if (i == moons.Count() - 1) line = '⎩';
                    Console.Write($"{line} \x1b[38;5;{moon.Color}m{moon.Name}\x1b[0m");
                }
                row++;
            }
        }
    }


}
