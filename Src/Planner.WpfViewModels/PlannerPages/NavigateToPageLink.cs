using System.Text.RegularExpressions;
using NodaTime;

namespace Planner.WpfViewModels.PlannerPages
{
    public static class NavigateToPageLink
    {
        /// <summary>
        /// This takes the group collection from a regex containing 3 or 4 matched integer values
        /// and navigates to the proper date using the month.day.note or month.day.year.note conventions
        /// </summary>
        public static void NavigateToDate(this IPlannerNavigator nav, GroupCollection groups,
            LocalDate today)
        {
            nav.ToDate(new LocalDate(GetLinkYear(groups, today), GetSegmentValue(1, groups),
                GetSegmentValue(2,groups))); 
        }

        private static int GetLinkYear(GroupCollection groups, in LocalDate today)
        {
            return groups.Count == 5 ? GetLinkYear(groups[3].Value, today):today.Year;
        }
        private static int GetLinkYear(string yearString, LocalDate today)
        {
            var rawYearIndicator = int.Parse(yearString);
            return rawYearIndicator < 100 ? rawYearIndicator + CurrentCentury(today) : rawYearIndicator;
        }

        private static int CurrentCentury(LocalDate date) => date.Year - (date.Year % 100);

        private static int GetSegmentValue(int index, GroupCollection coll) => 
            int.Parse(coll[index].Value);
    }
}