namespace RobotTest.Entities.Assignments;

/// <summary>
/// Задание на зонирование
/// </summary>
public class AssignmentItem
{
    /// <summary>
    /// Название размещаемого объекта
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Минимальное значение по Сх
    /// </summary>
    public float MinCx { get; set; }
    
    /// <summary>
    /// Максимальное значение по Сх
    /// </summary>
    public float MaxCx { get; set; }
    
    /// <summary>
    /// Шаги по Сх
    /// </summary>
    public float[] StepsCx { get; set; }

    /// <summary>
    /// Минимальное значение по Сy
    /// </summary>
    public float MinCy { get; set; }
    
    /// <summary>
    /// Максимальное значение по Сy
    /// </summary>
    public float MaxCy { get; set; }
    
    /// <summary>
    /// Шаги по Сy
    /// </summary>
    public float[] StepsCy { get; set; }

    /// <summary>
    /// Минимальная площадь зоны
    /// </summary>
    public float MinArea { get; set; }
    
    /// <summary>
    /// Максимальная площадь зоны
    /// </summary>
    public float MaxArea { get; set; }

    /// <summary>
    /// Количество
    /// </summary>
    public int Count { get; set; } = -1;
}