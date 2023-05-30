using RobotTest.Common.Dto;

namespace RobotTest.Entities.Assignments;

public static class AssignmentBuilder
{
    public static Assignment GetAssignment(ModuleDataDto dto)
    {
        var propertiesAsAssignments = dto.Properties.Where(x => x.Code == "morph_cell_prop");
        
        var assignmentItems = propertiesAsAssignments.Select(x => new AssignmentItem
        {
            Name = x.Name,
            MinCx = float.Parse(x.Children.First(p => p.Code == "cx").Value.Split("|")[0].Trim()),
            MaxCx = float.Parse(x.Children.First(p => p.Code == "cx").Value.Split("|")[1].Trim()),
            MinCy = float.Parse(x.Children.First(p => p.Code == "cy").Value.Split("|")[0].Trim()),
            MaxCy = float.Parse(x.Children.First(p => p.Code == "cy").Value.Split("|")[1].Trim()),
            MinArea = float.Parse(x.Children.First(p => p.Code == "s").Value.Split("|")[0].Trim()),
            MaxArea = float.Parse(x.Children.First(p => p.Code == "s").Value.Split("|")[1].Trim()),
            StepsCx = x.Children
                .First(p => p.Code == "cx_steps").Value
                .TrimStart('[')
                .TrimEnd(']')
                .Split(",")
                .Select(v => float.Parse(v.Trim()))
                .ToArray(),
            StepsCy = x.Children
                .First(p => p.Code == "cy_steps").Value
                .TrimStart('[')
                .TrimEnd(']')
                .Split(",")
                .Select(v => float.Parse(v.Trim()))
                .ToArray(),
        }).ToArray();

        return new Assignment
        {
            Items = assignmentItems
        };
    }
}