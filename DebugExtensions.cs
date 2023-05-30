using System.Globalization;
using System.Text;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class DebugExtensions
{
    public static string GetInfo(this Polygon polygon)
    {
        var stringBuilder = new StringBuilder();
        stringBuilder.Append(
            $"{{\"type\": \"FeatureCollection\",\"name\": \"Kazan-25m\",\"features\": [{{ \"type\": \"Feature\", \"properties\": {{ \"Name\": \"test\", \"ID\": \"{Random.Shared.Next()}\", \"area\": 57096028206669 }}, \"geometry\":{{ \"type\": \"Polygon\", \"coordinates\": [ [ ");
        foreach (var point in polygon.Points)
        {
            stringBuilder.Append($"[{point.X.ToString("g", CultureInfo.InvariantCulture)}, {point.Y.ToString("g",CultureInfo.InvariantCulture)}]");
            stringBuilder.Append(", ");
        }

        stringBuilder.Remove(stringBuilder.Length - 2, 2);

        stringBuilder.Append("] ] } }]}");

        return stringBuilder.ToString();
    }

    public static string GetInfoEdges(this Polygon polygon)
    {
        var stringBuilder = new StringBuilder();

        foreach (var edge in polygon.Edges)
        {
            stringBuilder.AppendLine($"{edge.Start} | {edge.End} sin: {edge.Sin}");
        }

        return stringBuilder.ToString();
    }
}