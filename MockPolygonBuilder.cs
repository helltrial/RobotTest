using RobotTest.Common.Dto;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class MockPolygonBuilder
{
    public static Polygon GetPolygon(ModuleDataDto dto)
    {
        var edges = new[]
        {
            new Edge
            {
                Start = new Point
                {
                    X = 5,
                    Y = 0
                },
                End = new Point
                {
                    X = 0,
                    Y = 10
                },
                UdsPriority = 4
            },
            new Edge
            {
                Start = new Point
                {
                    X = 0,
                    Y = 10
                },
                End = new Point
                {
                    X = 14,
                    Y = 10
                },
                UdsPriority = 3
            },
            new Edge
            {
                Start = new Point
                {
                    X = 14,
                    Y = 10
                },
                End = new Point
                {
                    X = 13,
                    Y = 0
                },
                UdsPriority = 2
            },
            new Edge
            {
                Start = new Point
                {
                    X = 13,
                    Y = 0
                },
                End = new Point
                {
                    X = 5,
                    Y = 0
                },
                UdsPriority = 1
            }
        };
        
        return new Polygon(edges, PolygonType.Abstract);
    }
}