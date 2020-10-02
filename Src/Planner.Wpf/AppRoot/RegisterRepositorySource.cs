using System.Net.Http;
using Melville.IOC.IocContainers;
using Planner.Models.Repositories;
using Planner.Repository.SqLite;
using Planner.Repository.Web;
using Planner.WpfViewModels.Logins;

namespace Planner.Wpf.AppRoot
{
    public class RegisterRepositorySource : IRegisterRepositorySource
    {
        private IocContainer container;

        public RegisterRepositorySource(IocContainer container)
        {
            this.container = container;
        }

        public void UseWebSource(HttpClient authenticatedClient)
        {
            container.Bind<IJsonWebService>().To<JsonWebService>().WithParameters(authenticatedClient);
            container.Bind<IPlannerTaskRemoteRepository>().To<PlannerTaskWebRepository>();
        }

        public void UseLocalTestSource()
        {
            var localDb = TestDatabaseFactory.TestDatabaseCreator();
            container.Bind<IPlannerTaskRemoteRepository>().To<SqlPlannerTaskRepository>()
                .WithParameters(localDb);
        }
    }
}