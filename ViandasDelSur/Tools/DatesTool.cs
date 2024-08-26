namespace ViandasDelSur.Tools
{
    public class DatesTool
    {
        public static DateTime GetNextDay(DayOfWeek day)
        {
            DateTime today = DateTime.Today;
            int daysUntilNextDay = ((int)day - (int)today.DayOfWeek + 7) % 7;

            if (daysUntilNextDay == 0)
                daysUntilNextDay = 8;

            DateTime nextDay = today.AddDays(daysUntilNextDay);

            return nextDay;
        }

        public static DateTime GetNextWeekDay(DayOfWeek day)
        {
            DateTime today = DateTime.Today;
            int daysUntilNextDay = ((int)day - (int)today.DayOfWeek + 7) % 7;

            if (daysUntilNextDay == 0)
                daysUntilNextDay = 8;
            else
                daysUntilNextDay += 8;

            DateTime nextDay = today.AddDays(daysUntilNextDay);

            return nextDay;
        } 
    }
}
