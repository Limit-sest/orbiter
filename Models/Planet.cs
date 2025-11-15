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

    private static Dictionary<(int X, int Y), List<PathPoint>> paths = new Dictionary<(int X, int Y), List<PathPoint>>();

    private string symbol;
    private string name;
    private int prevX = -1;
    private int prevY = -1;
    private int radius;
    private double pathPosition = 0;
    private int pathIndex = 0;
    private double speed;
    private int fg_color;
    private int bg_color;
    private List<PathPoint> path = new List<PathPoint>();

    public Planet(string symbol, string name, int radius, double speed, int fg_color, int bg_color)
    {
        this.symbol = symbol;
        this.name = name;
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

    public void DrawLabel(int offset, bool erase = false)
    {
        Console.SetCursorPosition(0, offset);
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
        var points = Helpers.EllipseGenerator.GenerateEllipse(Console.BufferWidth / 2, Console.BufferHeight / 2, (int)Math.Round(this.radius * 2.5), (int)Math.Round(this.radius * 0.8));
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
                Console.SetCursorPosition(prevX, prevY);
                Console.Write($"\x1b[38;5;{restoringPoint.Color}m{restoringPoint.Symbol}\x1b[0m");
            }
            ;
        }

        pathPosition += speed * AppState.SpeedMultiplier;
        pathIndex = ((int)pathPosition) % path.Count();

        var currentPoint = path[pathIndex];
        prevX = currentPoint.X;
        prevY = currentPoint.Y;

        Console.SetCursorPosition(currentPoint.X, currentPoint.Y);
        if (AppState.LabelsShown)
        {
            Console.Write($"\x1b[38;5;{fg_color}m{symbol}\x1b[0m");
        }
        else
        {
            Console.Write($"\x1b[38;5;{fg_color}m○\x1b[0m");
        }
    }

}
