using System.Net.Http;
using Melville.IOC.IocContainers;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
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
            container.Bind<IDatedRemoteRepository<PlannerTask>>().To<PlannerTasRemoteWebRepository>();
        }

        public void UseLocalTestSource()
        {
            var localDb = TestDatabaseFactory.TestDatabaseCreator();
            container.Bind<IDatedRemoteRepository<PlannerTask>>().To<SqlPlannerTasRemoteRepository>()
                .WithParameters(localDb);
        }
    }
}