using RobotTest.Common.Dto;
using RobotTest.Entities.Assignments;

namespace RobotTest;

public static class MockAssignmentBuilder
{
    public static Assignment GetAssignment(ModuleDataDto dto)
    {
        return new Assignment
        {
            Items = new[]
            {
                new AssignmentItem
                {
                    Name = "ЖГ",
                    MinCx = 2,
                    MinCy = 2,
                    MaxCx = 5,
                    MaxCy = 5,
                    MaxArea = 25,
                    MinArea = 4,
                    StepsCx = new float[]{3,4},
                    StepsCy = new float[]{3,4}
                }
            }
        };
    }
}