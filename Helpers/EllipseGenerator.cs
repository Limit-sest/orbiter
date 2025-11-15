namespace orbiter.Helpers;

public static class EllipseGenerator
{
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
}
