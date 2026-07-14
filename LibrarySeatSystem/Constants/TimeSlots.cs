namespace LibrarySeatSystem.Constants;

public static class TimeSlots
{
    public const string Morning1 = "08:00-10:00";
    public const string Morning2 = "10:00-12:00";
    public const string Afternoon1 = "14:00-16:00";
    public const string Afternoon2 = "16:00-18:00";
    public const string Evening = "19:00-21:00";

    public static readonly string[] All = new[]
    {
        Morning1, Morning2, Afternoon1, Afternoon2, Evening
    };
}
