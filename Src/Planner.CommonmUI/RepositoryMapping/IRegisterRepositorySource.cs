namespace Planner.CommonmUI.RepositoryMapping;

public interface IRegisterRepositorySource
{
    void UseWebSource(HttpClient authenticatedClient);
    void UseLocalTestSource();
}