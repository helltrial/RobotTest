using System.Globalization;

namespace RobotTest.Entities.Geometries;

public struct Point : IEquatable<Point>
{
    private const double Epsilon = 0.01;
    public float X { get; set; }
    public float Y { get; set; }

    public bool Equals(Point other)
    {
        return Math.Abs(X - other.X) <= Epsilon && Math.Abs(Y - other.Y) <= Epsilon;
    }

    public override bool Equals(object? obj)
    {
        return obj is Point other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public override string ToString()
    {
        return $"{X.ToString("g", CultureInfo.InvariantCulture)}:{Y.ToString("g", CultureInfo.InvariantCulture)}";
    }
}