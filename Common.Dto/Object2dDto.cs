namespace RobotTest.Common.Dto;

public class Object2dDto
{
    public string Id { get; set; } = null!; // Borukhina KO string из-за несоответствия формату прототипов Аффинум. Здесь будет Guid 

    public string Name { get; set; } = string.Empty;

    public string? Code { get; set; } = string.Empty;

    public string LayerCode { get; set; } = null!;

    public GeometryDto? Geometry { get; set; }

    public bool? Options { get; set; }

    public List<PropertyDto> Properties { get; set; } = new();

    public List<Object2dDto> Children { get; set; } = new();

    public Object2dDto GetCopy()
    {
        var children = new List<Object2dDto>();
        Children.ForEach(child =>
        {
            children.Add(child.GetCopy());
        });

        var properties = new List<PropertyDto>();
        Properties.ForEach(element =>
        {
            properties.Add(element.GetCopy());
        });

        return new Object2dDto
        {
            Id = Id,
            Name = Name,
            LayerCode = LayerCode,
            Options = Options,
            Code = Code,
            Geometry = Geometry?.GetCopy(),
            Properties = properties,
            Children = children
        };
    }
}