namespace RobotTest.Entities.Geometries;

public class Polygon //todo ctor + save
{
    private const double Epsilon = 0.001;

    public Polygon()
    {
    }

    public Polygon(Edge[] edges, PolygonType type = PolygonType.Abstract)
    {
        CheckAndConstructEdges(edges);
        Type = type;
    }

    public bool IsEmpty => !Edges?.Any() ?? true;
    public Edge[] Edges { get; private set; }
    public PolygonType Type { get; set; }
    public Edge PriorityEdge => Edges?.MinBy(x => x.UdsPriority);

    /// <summary>
    /// Ориентация полигона по часовой/против часовой стрелки
    /// true - по часовой
    /// ref: https://shorturl.at/cfDVZ
    /// </summary>
    public bool IsClockwise => Edges.Sum(edge => edge.Start.X * edge.End.Y - edge.End.X * edge.Start.Y) <= 0;

    /// <summary>
    /// Точки полигона
    /// </summary>
    public Point[] Points => Edges.Select(x => x.Start).ToArray();

    /// <summary>
    /// Массив всех точек полигона. Используется для библиотеки Clipper
    /// </summary>
    public double[] PointsArray => Points.Select(x => new List<double> { x.X, x.Y })
        .Aggregate((result, item) =>
        {
            result.AddRange(item);
            return result;
        }).ToArray();

    private void CheckAndConstructEdges(Edge[] edges)
    {
        var edgeList = edges.ToList();
        var currentIndex = 0;
        var nextIndex = 1;
        while (currentIndex < edgeList.Count - 1)
        {
            if (Math.Abs(edgeList[currentIndex].Sin - edgeList[nextIndex].Sin) < Epsilon &&
                Math.Sign(edgeList[currentIndex].Cos) == Math.Sign(edgeList[nextIndex].Cos))
            {
                edgeList[currentIndex].End = edgeList[nextIndex].End;
                edgeList.RemoveAt(nextIndex);
            }
            else
            {
                currentIndex++;
                nextIndex++;
            }
        }

        if (Math.Abs(edgeList[0].Sin - edgeList[^1].Sin) < Epsilon &&
            Math.Sign(edgeList[0].Cos) == Math.Sign(edgeList[^1].Cos))
        {
            edgeList[^1].End = edgeList[0].End;
            edgeList.RemoveAt(0);
        }

        Edges = edgeList.ToArray();
    }
}