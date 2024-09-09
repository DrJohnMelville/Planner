using System.Windows.Input;

namespace Planner.Maui.Pages.Login;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginPageViewModel vm)
    {
        BindingContext = vm;
        InitializeComponent();
    }
}

public interface IAfterLoginOperation
{
    void Do();
}

public class LoginPageViewModel
{
    public ICommand DoneLoginCommand { get; }
    public LoginPageViewModel(IAfterLoginOperation after)
    {
        DoneLoginCommand = new Command(after.Do);
    }
}