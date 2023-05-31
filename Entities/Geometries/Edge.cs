namespace RobotTest.Entities.Geometries;

/// <summary>
/// Ребро полигона
/// </summary>
public class Edge
{
    public Point Start { get; set; }
    public Point End { get; set; }
    public int UdsPriority { get; set; }
    public string UdsType { get; set; }

    /// <summary>
    /// Направление
    /// </summary>
    public EdgeDirectionType DirectionType => Math.Abs(Start.Y - End.Y) >= Math.Abs(Start.X - End.X)
        ? EdgeDirectionType.Cy
        : EdgeDirectionType.Cx;

    /// <summary>
    /// Длина ребра
    /// </summary>
    public float Length => (float)
        Math.Sqrt(Math.Pow(Math.Abs(Start.X - End.X), 2) + Math.Pow(Math.Abs(Start.Y - End.Y), 2));

    /// <summary>
    /// Синус угла
    /// </summary>
    public float Sin => (float)((End.Y - Start.Y) / Length);
    
    /// <summary>
    /// Косинус угла
    /// </summary>
    public float Cos => (float)((End.X - Start.X) / Length);
    
    /// <summary>
    /// Тангенс угла
    /// </summary>
    public float Tan => (float)((End.Y - Start.Y) / (End.X - Start.X));

    public override string ToString()
    {
        return $"{Start} - {End}";
    }
}