using System.Text;
using Clipper2Lib;
using RobotTest.Entities.Assignments;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class ZoneService
{
    public static void Processing(PolygonContainerResult polygonContainerResult, Assignment assignment)
    {
        var variants = assignment.GetVariants(polygonContainerResult.Source.PriorityEdge);
        polygonContainerResult.InnerResult = new List<PolygonContainerResult>();

        if (!IsPolygonCanBeZoned(polygonContainerResult.Source, variants))
        {
            polygonContainerResult.InnerResult.Add(new PolygonContainerResult
            {
                Result = new[] { polygonContainerResult.Source }
            });
            return;
        }

        foreach (var variant in variants)
        {
            var result = GetPolygons(polygonContainerResult.Source, variant);
            SetAttributesFromSourcePolygon(polygonContainerResult.Source, result);

            var remainPolygon = GetRemainPolygon(polygonContainerResult.Source, result);
            if (remainPolygon.IsEmpty)
            {
                polygonContainerResult.InnerResult.Add(new PolygonContainerResult
                {
                    Result = result,
                    Variant = variant
                });
                return;
            }

            SetAttributesFromSourcePolygon(polygonContainerResult.Source, new[] { remainPolygon });
            polygonContainerResult.InnerResult.Add(new PolygonContainerResult
            {
                Result = result,
                Variant = variant,
                Source = remainPolygon
            });
            
            //Debug
            Console.WriteLine($"Source");
            Console.WriteLine(polygonContainerResult.Source.GetInfoEdges());
            Console.WriteLine("Source priority edge");
            Console.WriteLine(polygonContainerResult.Source.PriorityEdge);
            Console.WriteLine($"Variant: CX {variant.Cx} CY {variant.Cy} ZoneCount: {variant.ZoneCount}");
            Console.WriteLine();
            result.ToList().ForEach(x => Console.WriteLine(x.GetInfoEdges()));
            Console.WriteLine("Remain");
            Console.WriteLine(remainPolygon.GetInfoEdges());
            if (result.Any(x => x.Edges.Length != 4) || remainPolygon.Edges.Length != 4)
            {
                Console.WriteLine("Error");
            }
            //Debug
            
            var innerPolygonContainerResult = polygonContainerResult.InnerResult.First(x => x.Variant == variant);
            Processing(innerPolygonContainerResult, assignment);
        }
    }

    private static void SetAttributesFromSourcePolygon(Polygon sourcePolygon, Polygon[] result)
    {
        var sourceMaxUdsPriority = sourcePolygon.Edges.MaxBy(x => x.UdsPriority).UdsPriority;
        var newUdsPriority = sourceMaxUdsPriority;
        foreach (var targetPolygon in result)
        {
            foreach (var targetEdge in targetPolygon.Edges)
            {
                foreach (var sourceEdge in sourcePolygon.Edges)
                {
                    if (targetEdge.Start.Equals(sourceEdge.Start) && targetEdge.End.Equals(sourceEdge.End) ||
                        targetEdge.End.Equals(sourceEdge.Start) && targetEdge.Start.Equals(sourceEdge.End))
                    {
                        targetEdge.UdsPriority = sourceEdge.UdsPriority;
                        targetEdge.UdsType = sourceEdge.UdsType;
                        break;
                    }

                    if ((targetEdge.Start.Equals(sourceEdge.Start) ||
                         targetEdge.Start.Equals(sourceEdge.End) ||
                         targetEdge.End.Equals(sourceEdge.Start) ||
                         targetEdge.End.Equals(sourceEdge.End)) &&
                        Math.Abs(targetEdge.Sin) == Math.Abs(sourceEdge.Sin))
                    {
                        targetEdge.UdsPriority = sourceEdge.UdsPriority;
                        targetEdge.UdsType = sourceEdge.UdsType;
                        break;
                    }
                }

                if (targetEdge.UdsPriority == 0)
                {
                    targetEdge.UdsPriority = newUdsPriority;
                    newUdsPriority++;
                }
            }

            newUdsPriority = sourceMaxUdsPriority;
        }
    }

    private static bool IsPolygonCanBeZoned(Polygon sourcePolygon, List<AssignmentVariant> variants)
    {
        if (!variants.Any())
        {
            return false;
        }

        var polygonArea = Math.Abs(Clipper.Area(Clipper.MakePath(sourcePolygon.PointsArray)));

        return variants.Any(variant => variant.Area <= polygonArea);
    }

    private static Polygon GetRemainPolygon(Polygon sourcePolygon, Polygon[] processedPolygons)
    {
        var unionPolygon = new PathsD();
        foreach (var processedPolygon in processedPolygons)
        {
            var pathsD = new PathsD { Clipper.MakePath(processedPolygon.PointsArray) };
            if (!unionPolygon.Any())
            {
                unionPolygon = pathsD;
            }
            else
            {
                unionPolygon = Clipper.Union(unionPolygon,
                    new PathsD { Clipper.MakePath(processedPolygon.PointsArray) }, FillRule.NonZero);
            }
        }
        
        var dif = Clipper.Difference(new PathsD { Clipper.MakePath(sourcePolygon.PointsArray) }, unionPolygon,
            FillRule.NonZero);
        return dif.GetPolygonFromPathsD();
    }

    private static Polygon[] GetPolygons(Polygon polygon, AssignmentVariant variant)
    {
        var currentEdge = polygon.PriorityEdge;
        var points = GetPointsOnEdge(currentEdge, variant);

        var result = new Polygon[points.Count - 1];
        for (var index = 0; index < points.Count - 1; index++)
        {
            var edge = new Edge
            {
                Start = points[index],
                End = points[index + 1],
                UdsPriority = currentEdge.UdsPriority,
                UdsType = currentEdge.UdsType
            };
            var length = edge.DirectionType == EdgeDirectionType.Cx ? variant.Cy : variant.Cx;
            var rectangle = GetRectangle(edge, length, polygon.IsClockwise);
            result[index] = GetPolygonFromRectangle(polygon, rectangle);
        }

        return result;
    }

    private static Polygon GetPolygonFromRectangle(Polygon source, Polygon target)
    {
        var sourcePath = new PathsD { Clipper.MakePath(source.PointsArray) };
        var targetPath = new PathsD { Clipper.MakePath(target.PointsArray) };

        var intersection = Clipper.Intersect(sourcePath, targetPath, FillRule.NonZero);

        return intersection.GetPolygonFromPathsD();
    }

    private static Polygon GetRectangle(Edge edge, float length, bool isClockwisePolygon)
    {
        var pointStartPerpendicular = GetPerpendicularPoint(edge.Start, edge, length, isClockwisePolygon);
        var pointEndPerpendicular = GetPerpendicularPoint(edge.End, edge, length, isClockwisePolygon);

        var edges = new[]
        {
            edge,
            new Edge
            {
                Start = edge.End,
                End = pointEndPerpendicular
            },
            new Edge
            {
                Start = pointEndPerpendicular,
                End = pointStartPerpendicular
            },
            new Edge
            {
                Start = pointStartPerpendicular,
                End = edge.Start
            }
        };

        return new Polygon(edges);
    }

    private static Point GetPerpendicularPoint(Point point, Edge edge, float length, bool isClockwisePolygon)
    {
        var Sx = length * edge.Sin;
        var Sy = length * edge.Cos;

        return isClockwisePolygon switch
        {
            true => new Point { X = point.X + Sx, Y = point.Y - Sy },
            false => new Point { X = point.X - Sx, Y = point.Y + Sy }
        };
    }

    private static List<Point> GetPointsOnEdge(Edge edge, AssignmentVariant variant)
    {
        var distanceBetweenPoints = edge.Length / variant.ZoneCount;

        // Вычисляем длины проекции на ось X и ось Y ребра edge
        var Dx = edge.End.X - edge.Start.X;
        var Dy = edge.End.Y - edge.Start.Y;

        // Вычисляем длины проекций на ось X и ось Y ребра edge длины distanceBetweenPoints //todo 2 вершины могут совпасть
        var dx = Dx * distanceBetweenPoints / edge.Length;
        var dy = Dy * distanceBetweenPoints / edge.Length;

        // Точка дотягивания слева для формирования доп полигонов, чьи перпендикуляры не пересекают ребра //todo один из двух возможных сценариев
        var result = new List<Point>
        {
            new()
            {
                X = edge.Start.X - (dx * 2),
                Y = edge.Start.Y - (dy * 2)
            }
        };

        // Вычисляем точки на ребре edge, используя смещение dx и dy
        for (var i = 1; i <= variant.ZoneCount - 1; i++)
        {
            var point = new Point
            {
                X = edge.Start.X + (dx * i),
                Y = edge.Start.Y + (dy * i)
            };
            result.Add(point);
        }

        // Точка дотягивания справа для формирования доп полигонов, чьи перпендикуляры не пересекают ребра
        result.Add(new Point
        {
            X = edge.End.X + (dx * 2),
            Y = edge.End.Y + (dy * 2)
        });

        return result;
    }
}