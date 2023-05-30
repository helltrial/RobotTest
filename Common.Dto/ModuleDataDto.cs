namespace RobotTest.Common.Dto;

public class ModuleDataDto
{
    public List<PropertyDto> Properties { get; set; } = new();
    
    /// <summary>
    /// Объёдинённые данные 2D+3D.
    /// </summary>
    public List<Object2dDto> Data { get; set; } = new();

    public List<Object2dDto> GetDataCopy()
    {
        var result = new List<Object2dDto>();
        Data.ForEach(element =>
        {
            result.Add(element.GetCopy());
        });
        return result;
    }

    public List<PropertyDto> GetPropertiesCopy()
    {
        var result = new List<PropertyDto>();
        Properties.ForEach(element =>
        {
            result.Add(element.GetCopy());
        });
        return result;
    }
}