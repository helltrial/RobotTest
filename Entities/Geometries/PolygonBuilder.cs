using RobotTest.Common.Dto;

namespace RobotTest.Entities.Geometries;

public static class PolygonBuilder
{
    private const string Area = "area";
    private const string AreaBlock = "area_block";
    private const string UdsContainer = "uds_container";
    private const string UdsLine = "uds_line";
    private const string UdsType = "uds_type";
    private const string UdsProperty = "uds_prop";
    private const string UdsPriority = "uds_priority";

    public static Polygon GetPolygon(ModuleDataDto dto)
    {
        var edgeNamesString = dto.Data
            .First(x => x.Code == Area).Children
            .First(x => x.Code == AreaBlock).Properties
            .First(x => x.Code == UdsContainer).Value;
        var edgeNames = edgeNamesString.Split("; ");

        var udsLines = dto.Data.First(x => x.Code == UdsLine)
            .Children.Where(x => x.Code == UdsLine).IntersectBy(edgeNames, x => x.Id).ToArray();

        var edgesDifDirection = udsLines.Select(x => new Edge
        {
            Start = new Point { X = (float)x.Geometry.Coordinates[0][0], Y = (float)x.Geometry.Coordinates[0][1] },
            End = new Point { X = (float)x.Geometry.Coordinates[1][0], Y = (float)x.Geometry.Coordinates[1][1] },
            UdsType = x.Properties.First(p => p.Code == UdsType).Value
        }).ToArray();

        var edges = SetDirection(edgesDifDirection);

        var udsPropertyNamePriorities = dto.Properties.Where(x => x.Code == UdsProperty)
            .ToDictionary(k => k.Name, v => int.Parse(v.Children.First(x => x.Code == UdsPriority).Value));

        for (var i = 0; i < edges.Length; i++)
        {
            edges[i].UdsPriority = udsPropertyNamePriorities[edges[i].UdsType];
        }

        return new Polygon(edges);
    }

    /// <summary>
    /// Метод для перенаправления ребер в одну сторону
    /// </summary>
    /// <param name="edges">Исходный набор ребер</param>
    /// <returns>Результирующий набор ребер</returns>
    private static Edge[] SetDirection(Edge[] edges)
    {
        var edgeList = edges.Skip(1).ToList();
        var result = new List<Edge> { edges[0] };

        while (edgeList.Any())
        {
            var lastPoint = result.Last().End;
            var nextRightDirectionEdge = edgeList.FirstOrDefault(x => x.Start.Equals(lastPoint));
            if (nextRightDirectionEdge != null)
            {
                result.Add(nextRightDirectionEdge);
                edgeList.Remove(nextRightDirectionEdge);
                continue;
            }

            var nextWrongDirectionEdge = edgeList.First(x => x.End.Equals(lastPoint));

            (nextWrongDirectionEdge.Start, nextWrongDirectionEdge.End) = (nextWrongDirectionEdge.End, nextWrongDirectionEdge.Start);
            result.Add(nextWrongDirectionEdge);
            edgeList.Remove(nextWrongDirectionEdge);
        }
        
        return result.ToArray();
    }
}