using RobotTest.Entities.Assignments;

namespace RobotTest.Entities.Geometries;

public class PolygonContainerResult
{
    public Polygon Source { get; set; }
    public Polygon[] Result { get; set; }
    public AssignmentVariant Variant { get; set; }
    public List<PolygonContainerResult> InnerResult { get; set; } = new();
}