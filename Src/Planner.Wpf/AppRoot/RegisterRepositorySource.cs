using System.Net.Http;
using Melville.IOC.IocContainers;
using Planner.Models.Blobs;
using Planner.Models.Notes;
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
            RegisterWebRepository<PlannerTask>("/Task");
            RegisterWebRepository<Note>("/Note");
        }

        private void RegisterWebRepository<T>(string urlPrefix) where T:PlannerItemWithDate => 
            container.Bind<IDatedRemoteRepository<T>>().To<WebRepository<T>>().WithParameters(urlPrefix);

        public void UseLocalTestSource()
        {
            var localDb = TestDatabaseFactory.TestDatabaseCreator();
            container.Bind<IDatedRemoteRepository<PlannerTask>>().To<SqlRemoteRepositoryWithDate<PlannerTask>>()
                .WithParameters(localDb);
            container.Bind<IDatedRemoteRepository<Note>>().To<SqlRemoteRepositoryWithDate<Note>>()
                .WithParameters(localDb);
            container.Bind<IDatedRemoteRepository<Blob>>().To<SqlRemoteRepositoryWithDate<Blob>>();
        }
    }
}