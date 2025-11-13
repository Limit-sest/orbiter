namespace orbiter;

class Program
{
    class Sun
    {
        public void DrawOnce()
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

                currentPoint.Symbol = (previousPoint.X.CompareTo(nextPoint.X), previousPoint.Y.CompareTo(nextPoint.Y)) switch
                {
                    (1, -1) => '┘',
                    (-1, -1) => '┐',
                    (1, 1) => '└',
                    (-1, 1) => '┌',
                    (1, 0) => '─',
                    (-1, 0) => '─',
                    (0, 1) => '│',
                    (0, -1) => '│',

                    _ => 'x'
                };
            }
        }

        public void DrawPathOnce()
        {
            foreach (PathPoint point in path)
            {
                Console.SetCursorPosition(point.X, point.Y);
                Console.Write(point.Symbol);
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
            // Erase previous position by redrawing the path character at that location
            if (prevX >= 0 && prevY >= 0)
            {
                // Find the path point at previous position and redraw it
                var pathPoint = path.FirstOrDefault(p => p.X == prevX && p.Y == prevY);
                if (pathPoint != null)
                {
                    Console.SetCursorPosition(prevX, prevY);
                    Console.Write(pathPoint.Symbol);
                }
            }

            // Calculate new position
            (this.x, this.y) = GetPos(this.angle, this.radius);
            angle -= speed;
            if (angle <= 0)
            {
                angle = 360;
            }

            // Draw planet at new position
            Console.SetCursorPosition(this.x, this.y);
            Console.Write(symbol);

            // Store current position as previous for next frame
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
        Console.Write("\x1b[
