namespace RobotTest.Common.Dto;

public class PropertyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string Value { get; set; } = string.Empty;
    public int Type { get; set; } // Текст (0), Целое число (1), Вещественное число (2), Интервал (3), Логический (4). Может быть не задан. По умолчанию "Текст"
    public string? Unit { get; set; }
    
    public List<PropertyDto>? Children { get; set; }

    public PropertyDto GetCopy()
    {
        var children = new List<PropertyDto>();
        Children?.ForEach(child =>
        {
            children.Add(child.GetCopy());
        });

        return new PropertyDto
        {
            Id = Id,
            Name = Name,
            Code = Code,
            Value = Value,
            Type = Type,
            Children = children
        };
    }
}