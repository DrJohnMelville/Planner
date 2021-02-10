using System;
using System.Collections.Generic;
using Melville.MVVM.Wpf.RootWindows;
using NodaTime;

namespace Planner.Wpf.PlannerPages
{

    public class PlannerPageNavigationHistory: INavigationHistory
    {
        private readonly Stack<LocalDate> priorPages = new Stack<LocalDate>();
        private readonly Func<LocalDate, DailyPlannerPageViewModel> pageFactory;

        public PlannerPageNavigationHistory(Func<LocalDate, DailyPlannerPageViewModel> pageFactory)
        {
            this.pageFactory = pageFactory;
        }

        public object? Pop() => priorPages.TryPop(out var date) ? pageFactory(date) : null;

        public void Push(object content)
        {
            if (content is DailyPlannerPageViewModel dpp)
            {
                priorPages.Push(dpp.CurrentDate);
            }
        }
    }
}