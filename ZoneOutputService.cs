using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class ZoneOutputService
{
    public static List<List<Polygon>> GetResult(PolygonContainerResult polygonContainerResult)
    {
        var result = new List<List<Polygon>>();
        SetInnerResult(polygonContainerResult, ref result);

        return result;
    }

    private static void SetInnerResult(PolygonContainerResult containerResult, ref List<List<Polygon>> innerResult)
    {
        if (containerResult.InnerResult.Any())
        {
            foreach (var containerInnerResult in containerResult.InnerResult)
            {
                SetInnerResult(containerInnerResult, ref innerResult);
            }
        }
        else
        {
            innerResult.Add(new List<Polygon>());
            SetHeadResult(containerResult, innerResult.Last());
        }
        
    }

    private static void SetHeadResult(PolygonContainerResult containerResult, List<Polygon> result)
    {
        while (true)
        {
            if (containerResult.Result?.Any() == true)
            {
                result.AddRange(containerResult.Result);
            }

            if (containerResult.Parent != null)
            {
                containerResult = containerResult.Parent;
                continue;
            }

            break;
        }
    }
}