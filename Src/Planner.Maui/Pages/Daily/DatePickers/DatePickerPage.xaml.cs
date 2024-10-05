using Melville.INPC;
using NodaTime;

namespace Planner.Maui.Pages.Daily.DatePickers;

public partial class DatePickerPage : ContentPage
{
	public DatePickerViewModel ViewModel { get; } = new DatePickerViewModel();
    public DatePickerPage()
	{
		InitializeComponent();
        BindingContext = ViewModel;
    }
}

