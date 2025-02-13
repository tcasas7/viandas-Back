namespace ViandasDelSur.Tools
{
    public class DatesTool
    {
        public static DateTime GetNextDay(DayOfWeek day)
        {
            DateTime today = DateTime.Today;
            int daysUntilNextDay = ((int)day - (int)today.DayOfWeek + 7) % 7;

            // Si es el mismo día, saltar a la próxima semana (7 días)
            if (daysUntilNextDay == 0)
                daysUntilNextDay = 7;

            DateTime nextDay = today.AddDays(daysUntilNextDay);
            return nextDay;
        }

        public static DateTime GetNextWeekDay(DateTime date)
        {
            // Si la fecha es sábado o domingo, avanzar hasta el próximo lunes
            while (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday)
            {
                date = date.AddDays(1);
            }
            return date;
        }

    }
}
