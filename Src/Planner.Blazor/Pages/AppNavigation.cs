using Microsoft.AspNetCore.Components;
using NodaTime;

namespace Planner.Blazor.Pages
{
    public interface IAppNavigation
    {
        void ToPlannerPage(LocalDate date);
    }
    public class AppNavigation: IAppNavigation
    {
        private readonly NavigationManager nav;

        public AppNavigation(NavigationManager nav)
        {
            this.nav = nav;
        }

        public void ToPlannerPage(LocalDate date)
        {
            nav.NavigateTo($"/DailyPage/{date:yyyy-MM-dd}");
        }
    }
}