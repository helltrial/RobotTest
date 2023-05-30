namespace RobotTest.Entities.Assignments;

/// <summary>
/// Вариант размещения
/// </summary>
public class AssignmentVariant
{
    /// <summary>
    /// Название располагаемого объекта
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Длина по Cx
    /// </summary>
    public float Cx { get; set; }

    /// <summary>
    /// Высота по Cy
    /// </summary>
    public float Cy { get; set; }

    /// <summary>
    /// Площадь объекта зонирования
    /// </summary>
    public float Area => Cx * Cy;

    /// <summary>
    /// Количество зон
    /// </summary>
    public int ZoneCount { get; set; }
}