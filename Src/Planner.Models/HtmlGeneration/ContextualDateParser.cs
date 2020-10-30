using NodaTime;

namespace Planner.Models.HtmlGeneration
{
    public static class ContextualDateParser
    {
        public static LocalDate SelectedDate(string year, string month, string day, LocalDate baseDate) => 
            SelectedDate(ParseYear(year), int.Parse(month), int.Parse(day), baseDate);

        public static LocalDate SelectedDate(int year, int month, int day, LocalDate baseDate) =>
            new LocalDate(ComputeYear(baseDate, year), month, day);

        private static int ParseYear(string year) =>
            int.TryParse(year, out var ret) ? ret : -1;

        private static int ComputeYear(LocalDate baseDate, int year) =>
            year switch
            {
                < 0 => baseDate.Year,
                <100 => year + Century(baseDate.Year),
                _=> year
            };

        private static int Century(in int year)
        {
            return year - (year % 100);
        }
    }
}