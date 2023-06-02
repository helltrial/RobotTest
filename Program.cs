using Clipper2Lib;
using Newtonsoft.Json;
using RobotTest.Common.Dto;
using RobotTest.Entities.Assignments;
using RobotTest.Entities.Geometries;

namespace RobotTest;

public static class Program
{
    /*public static void Main(string[] args)
    {
        Paths64 subj = new Paths64();
        Paths64 clip = new Paths64();
        Paths64 temp = new Paths64();
        subj.Add(Clipper.MakePath(new[] { 0, 0, 2, 7, 10, 7, 10, 0 }));
        clip.Add(Clipper.MakePath(new[] { 0, 0, 0, 3, 3, 3, 3, 0 }));
        temp.Add(Clipper.MakePath(new[] { 3, 0, 3, 3, 6, 3, 6, 0 }));
        Paths64 dif = Clipper.Difference(subj, clip, FillRule.NonZero);

        Paths64 testIntersect = Clipper.Intersect(subj, clip, FillRule.NonZero);

        Paths64 testUnion = Clipper.Union(clip, temp, FillRule.NonZero);

        var unionIntersectionTest = Clipper.Difference(subj, testUnion, FillRule.NonZero);
        var dif1 = Clipper.Difference(dif, temp, FillRule.NonZero);
    }*/

    /*public static void Main(string[] ars) //Nettopology
    {
        var polyTestSource = new Polygon(new LinearRing(new[]
        {
            new Coordinate(0, 0), new Coordinate(2, 7), new Coordinate(10, 7), new Coordinate(10, 0),
            new Coordinate(0, 0)
        }));

        var polyTestTarget = new Polygon(new LinearRing(new[]
        {
            new Coordinate(0, 0), new Coordinate(0, 3), new Coordinate(3, 3), new Coordinate(3, 0),
            new Coordinate(0, 0)
        }));
        var polyTest2Target = new Polygon(new LinearRing(new[]
        {
            new Coordinate(3, 0), new Coordinate(6, 0), new Coordinate(6, 3), new Coordinate(3, 3), new Coordinate(3, 0)
        }));
        var dif = polyTestSource.Intersection(polyTestTarget) as Polygon;

        var result = polyTestSource.Difference(dif) as Polygon;
        result = result.Difference(polyTest2Target) as Polygon;
    }*/


    public static async Task Main(string[] args)
    {
        using var stream = new FileStream("input.json", FileMode.Open);
        using var streamReader = new StreamReader(stream);
        var jsonStr = await streamReader.ReadToEndAsync();
        var model = JsonConvert.DeserializeObject<ModuleDataDto>(jsonStr);

        var assignment = AssignmentBuilder.GetAssignment(model);
        var polygon = PolygonBuilder.GetPolygon(model);

        var containerResult = new PolygonContainerResult { Source = polygon };
        ZoneService.Processing(containerResult, assignment);

        var result = ZoneOutputService.GetResult(containerResult);
        
        
    }
}