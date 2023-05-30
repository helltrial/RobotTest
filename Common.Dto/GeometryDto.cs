namespace RobotTest.Common.Dto;

public class GeometryDto
{
    public string Type { get; set; } = null!;

    public double[][] Coordinates { get; set; } = Array.Empty<double[]>(); // X, Y, Z

    public GeometryPropertiesDto? Properties { get; set; }

    public GeometryDto GetCopy()
    {
        return new GeometryDto
        {
            Type = Type,
            Coordinates = Coordinates,
            Properties = Properties?.GetCopy(),
        };
    }
}

public static class GeometryType
{
    public const string Points = "Points";
    public const string Polyline = "Polyline";
    public const string Polygon = "Polygon";
    public const string Circle = "Circle";

    public static readonly IReadOnlyCollection<string> List = new[]
    {
        Points,
        Polyline,
        Polygon,
        Circle
    };
}