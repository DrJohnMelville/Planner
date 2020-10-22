using NodaTime;

namespace Planner.Models.HtmlGeneration
{
    public static class ContextualDateParser
    {
        public static LocalDate SelectedDate(string year, string month, string day, LocalDate baseDate)
        {
            return new LocalDate(ComputeYear(year, baseDate), int.Parse(month), int.Parse(day));
        }

        private static int ComputeYear(string year, in LocalDate baseDate)
        {
            if (!int.TryParse(year, out var intVal)) return baseDate.Year;
            return intVal < 100 ? intVal + Century(baseDate.Year) : intVal;
        }

        private static int Century(in int year)
        {
            return year - (year % 100);
        }
    }
}