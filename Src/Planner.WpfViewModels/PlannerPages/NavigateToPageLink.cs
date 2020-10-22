using System.Text.RegularExpressions;
using NodaTime;
using Planner.Models.HtmlGeneration;

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
            nav.ToDate(ContextualDateParser.SelectedDate(groups.Count==5?groups[3].Value:"",
                groups[1].Value, groups[2].Value, today));
        }
    }
}