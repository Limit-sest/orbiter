namespace orbiter;

public static class AppState
{
    public static bool Running { get; set; } = true;
    public static double SpeedMultiplier { get; set; } = 1.0;
    public static double OriginalSpeed { get; set; } = 1.0;
    public static bool LabelsShown { get; set; } = true;
    public static bool MoonsShown { get; set; } = true;
}
