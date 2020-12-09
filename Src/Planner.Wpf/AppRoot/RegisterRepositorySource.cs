using System;
using System.Net.Http;
using Melville.IOC.IocContainers;
using Melville.MVVM.FileSystem;
using Planner.Models.Appointments;
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
            container.Bind<IJsonWebService>().To<JsonWebService>()
                .WithParameters(authenticatedClient);
            RegisterWebRepository<PlannerTask>("/Task");
            RegisterWebRepository<Note>("/Note");
            RegisterWebRepository<Blob>("/Blob");
            container.Bind<INoteSearcher>().To<WebNoteSearcher>();
            container.Bind<IBlobContentStore>().To<WebBlobContentStore>()
                .WithParameters(authenticatedClient)
                .AsSingleton();
        }

        private void RegisterWebRepository<T>(string urlPrefix) where T:PlannerItemWithDate => 
            container.Bind<IDatedRemoteRepository<T>>().To<WebRepository<T>>().WithParameters(urlPrefix);

        public void UseLocalTestSource()
        {
            container.Bind<Func<PlannerDataContext>>().ToConstant(TestDatabaseFactory.TestDatabaseCreator());
            container.Bind<IDatedRemoteRepository<Blob>>().To <SqlRemoteRepositoryWithDate<Blob>>();
            container.Bind<IDatedRemoteRepository<Blob>>().To <CompopsiteBlobRemoteRepository>()
                .BlockSelfInjection();
            container.Bind<IDatedRemoteRepository<Appointment>>().To<AppointmentRemoteRepository>();
            container.BindGeneric(typeof(IDatedRemoteRepository<>), typeof(SqlRemoteRepositoryWithDate<>));
            container.Bind<IDirectory>().ToConstant(new MemoryDirectory("c:\\sss")).WhenConstructingType<BlobContentContentStore>();
            container.Bind<IBlobContentStore>().And<IDeletableBlobContentStore>()
                .To<BlobContentContentStore>().AsSingleton();
            container.Bind<INoteSearcher>().To<SqlNoteSearcher>();
        }
    }
}