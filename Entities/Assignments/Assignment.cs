using System.Xml.Schema;
using RobotTest.Entities.Geometries;

namespace RobotTest.Entities.Assignments;

/// <summary>
/// Контейнер данных для хранения заданий на зонирование
/// </summary>
public class Assignment
{
    /// <summary>
    /// Полное задания на зонирование
    /// </summary>
    public AssignmentItem[] Items { get; set; }

    public List<AssignmentVariant> GetVariants(Edge edge)
    {
        var item = Items[0];
        var resultVariants = new List<AssignmentVariant>();

        if (edge.DirectionType == EdgeDirectionType.Cx)
        {
            var minCount = (int)Math.Ceiling(edge.Length / item.MaxCx);
            var maxCount = (int)Math.Floor(edge.Length / item.MinCx);

            for (var counter = minCount; counter <= maxCount; counter++)
            {
                resultVariants.AddRange(item.StepsCy.Select(cy => new AssignmentVariant
                {
                    Cx = edge.Length / counter,
                    Cy = cy,
                    Name = item.Name,
                    ZoneCount = counter
                }));
            }
        }

        if (edge.DirectionType == EdgeDirectionType.Cy)
        {
            var minCount = (int)Math.Ceiling(edge.Length / item.MaxCy);
            var maxCount = (int)Math.Floor(edge.Length / item.MinCy);

            for (var counter = minCount; counter <= maxCount; counter++)
            {
                resultVariants.AddRange(item.StepsCx.Select(cx => new AssignmentVariant
                {
                    Cx = cx,
                    Cy = edge.Length / counter,
                    Name = item.Name,
                    ZoneCount = counter
                }));
            }
        }

        return resultVariants;
    }
}