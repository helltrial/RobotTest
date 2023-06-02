using RobotTest.Entities.Assignments;

namespace RobotTest.Entities.Geometries;

/// <summary>
/// Класс-хранилище результата зонирования
/// </summary>
public class PolygonContainerResult
{
    /// <summary>
    /// Исходный полигон для проведения зонирования
    /// </summary>
    public Polygon Source { get; set; }

    /// <summary>
    /// Полученный результат
    /// </summary>
    public Polygon[]? Result { get; set; }

    /// <summary>
    /// Варинат зонирования
    /// </summary>
    public AssignmentVariant? Variant { get; set; }

    /// <summary>
    /// Вложенные результаты
    /// </summary>
    public List<PolygonContainerResult> InnerResult { get; set; } = new();

    /// <summary>
    /// Голова 
    /// </summary>
    public PolygonContainerResult? Parent { get; set; }
}