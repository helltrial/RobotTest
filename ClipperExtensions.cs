using Clipper2Lib;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class ClipperExtensions
{
    public static PathsD GetPathsDFromPolygon(this Polygon polygon)
    {
        return new PathsD { Clipper.MakePath(polygon.PointsArray) };
    }
    public static Polygon GetPolygonFromPathsD(this PathsD pathsD)
    {
        if (!pathsD.Any())
        {
            return new Polygon();
        }
        
        var points = pathsD.First().Select(x => new Point
        {
            X = (float)x.x,
            Y = (float)x.y
        }).ToArray();
        var result = new List<Edge>();

        for (int i = 0; i < points.Length - 1; i++)
        {
            result.Add(new Edge
            {
                Start = points[i],
                End = points[i + 1]
            });
        }

        result.Add(new Edge
        {
            Start = points[^1],
            End = points[0]
        });

        return new Polygon(result.ToArray());
    }
}