namespace Planner.Maui.Pages.Daily;

public partial class DatePickerPage : ContentPage
{
	public DatePickerViewModel ViewModel { get; } = new DatePickerViewModel();
    public DatePickerPage()
	{
		InitializeComponent();
        BindingContext = ViewModel;
    }
}

public partial class DatePickerViewModel
{

}