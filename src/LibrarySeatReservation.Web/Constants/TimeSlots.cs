namespace LibrarySeatReservation.Web.Constants;

public static class TimeSlots
{
    public const string Morning1 = "Morning1";
    public const string Morning2 = "Morning2";
    public const string Afternoon1 = "Afternoon1";
    public const string Afternoon2 = "Afternoon2";
    public const string Evening = "Evening";

    public static readonly string[] All = { Morning1, Morning2, Afternoon1, Afternoon2, Evening };

    public static readonly Dictionary<string, string> DisplayNames = new()
    {
        { Morning1, "上午第一节 (08:00-10:00)" },
        { Morning2, "上午第二节 (10:00-12:00)" },
        { Afternoon1, "下午第一节 (14:00-16:00)" },
        { Afternoon2, "下午第二节 (16:00-18:00)" },
        { Evening, "晚上 (19:00-21:00)" }
    };
}
