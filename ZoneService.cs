using Clipper2Lib;
using RobotTest.Entities.Assignments;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class ZoneService
{
    private static float DiscriminantEpsilon = 0.01F;

    public static void Processing(PolygonContainerResult polygonContainerResult, Assignment assignment)
    {
        var variants = assignment.GetVariants(polygonContainerResult.Source.PriorityEdge);
        polygonContainerResult.InnerResult = new List<PolygonContainerResult>();

        if (!IsPolygonCanBeZoned(polygonContainerResult.Source, variants))
        {
            polygonContainerResult.InnerResult.Add(new PolygonContainerResult
            {
                Result = new[] { polygonContainerResult.Source },
                Parent = polygonContainerResult
            });
            return;
        }

        foreach (var variant in variants)
        {
            var (remainPolygon, result) = GetPolygons(polygonContainerResult.Source, variant);
            SetAttributesFromSourcePolygon(polygonContainerResult.Source, result);

            if (remainPolygon.IsEmpty)
            {
                polygonContainerResult.InnerResult.Add(new PolygonContainerResult
                {
                    Result = result,
                    Variant = variant,
                    Parent = polygonContainerResult
                });
                return;
            }

            SetAttributesFromSourcePolygon(polygonContainerResult.Source, new[] { remainPolygon });
            polygonContainerResult.InnerResult.Add(new PolygonContainerResult
            {
                Result = result,
                Variant = variant,
                Source = remainPolygon,
                Parent = polygonContainerResult
            });

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
                    // Совпадают обе вершины
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
                         targetEdge.End.Equals(sourceEdge.End)))
                    {
                        if (IsEdgesAreCollinear(sourceEdge, targetEdge))
                        {
                            targetEdge.UdsPriority = sourceEdge.UdsPriority;
                            targetEdge.UdsType = sourceEdge.UdsType;
                            break;
                        }
                    }
                }

                if (targetEdge.UdsPriority == 0)
                {
                    newUdsPriority++;
                    targetEdge.UdsPriority = newUdsPriority;
                }
            }

            newUdsPriority = sourceMaxUdsPriority;
        }
    }

    private static bool IsEdgesAreCollinear(Edge sourceEdge, Edge targetEdge)
    {
        var dxSource = sourceEdge.End.X - sourceEdge.Start.X;
        var dySource = sourceEdge.End.Y - sourceEdge.Start.Y;

        var dxTarget = targetEdge.End.X - targetEdge.Start.X;
        var dyTarget = targetEdge.End.Y - targetEdge.Start.Y;

        var discriminant = (dxSource * dyTarget - dySource * dxTarget);

        var temp = Math.Abs(discriminant) / 1000;

        // 100 - делитель, решающий ошибки округления
        return temp <= DiscriminantEpsilon;
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
            var pathsD = processedPolygon.GetPathsDFromPolygon();
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

        var dif = Clipper.Difference(sourcePolygon.GetPathsDFromPolygon(), unionPolygon,
            FillRule.NonZero);
        return dif.GetPolygonFromPathsD();
    }

    /// <summary>
    /// Возвращает результат выполнения нарезки по варианту
    /// </summary>
    /// <param name="polygon">Исходный полигон</param>
    /// <param name="variant">Вариант нарезки</param>
    /// <returns>Кортеж, где первым указан новый результирующий полигон, а вторым - массив результирующий полигонов, обработанных по варинту нарезки</returns>
    private static (Polygon, Polygon[]) GetPolygons(Polygon polygon, AssignmentVariant variant)
    {
        var currentEdge = polygon.PriorityEdge;
        var points = GetPointsOnEdge(currentEdge, variant);

        var result = new Polygon[points.Count - 1];
        var rectangles = new Polygon[points.Count - 1];
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
            var rectangleForIntersection = GetRectangle(edge, length, polygon.IsClockwise);

            result[index] = GetPolygonFromRectangle(polygon, rectangleForIntersection);

            rectangles[index] = GetRectangle(edge, length, polygon.IsClockwise, true);
        }

        var remainPolygon = GetRemainPolygon(polygon, rectangles);

        return (remainPolygon, result);
    }

    private static Polygon GetPolygonFromRectangle(Polygon source, Polygon target)
    {
        var sourcePath = new PathsD { Clipper.MakePath(source.PointsArray) };
        var targetPath = new PathsD { Clipper.MakePath(target.PointsArray) };

        var intersection = Clipper.Intersect(sourcePath, targetPath, FillRule.NonZero);

        return intersection.GetPolygonFromPathsD();
    }

    private static Polygon GetRectangle(Edge edge, float length, bool isClockwisePolygon, bool isOffset = false)
    {
        var startPoint = isOffset ? GetPerpendicularPoint(edge.Start, edge, length, !isClockwisePolygon) : edge.Start;
        var endPoint = isOffset ? GetPerpendicularPoint(edge.End, edge, length, !isClockwisePolygon) : edge.End;

        var pointStartPerpendicular = GetPerpendicularPoint(edge.Start, edge, length, isClockwisePolygon);
        var pointEndPerpendicular = GetPerpendicularPoint(edge.End, edge, length, isClockwisePolygon);

        var edges = new[]
        {
            new Edge
            {
                Start = startPoint,
                End = endPoint
            },
            new Edge
            {
                Start = endPoint,
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
                End = startPoint
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