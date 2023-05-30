namespace RobotTest.Common.Dto;

public class GeometryPropertiesDto
{
    public string? Color { get; set; }

    public double? Height { get; set; }

    public int? DivisionCount { get; set; }

    public GeometryPropertiesDto GetCopy()
    {
        return new GeometryPropertiesDto
        {
            Color = Color,
            Height = Height,
            DivisionCount = DivisionCount,
        };
    }
}