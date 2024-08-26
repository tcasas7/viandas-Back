using ViandasDelSur.Models;

namespace ViandasDelSur.Tools
{
    public class StatsTool
    {
        public static List<DateTime> GetDates(ICollection<SaleData> sales)
        {
            List<DateTime> dates = new List<DateTime>();

            foreach (SaleData sale in sales)
            {
                if (!dates.Contains(sale.validDate))
                {
                    dates.Add(sale.validDate);
                }
            }

            return dates;
        }
    }
}
