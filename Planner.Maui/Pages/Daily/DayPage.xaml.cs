namespace Planner.Maui.Pages.Daily;

public partial class DayPage : ContentPage
{
	public DayPage(DailyPageViewModel dpvm)
	{
		InitializeComponent();
		BindingContext = dpvm;
    }
}