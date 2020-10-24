using System;
using System.Threading.Tasks;
using Planner.Models.Blobs;
using Planner.Repository.SqLite;
using Xunit;

namespace Planner.Repository.Test.SqLite
{
    public class SqlItemByKeyRepositoryTest
    {
        private readonly TestDatabase data = new TestDatabase();
        private readonly SqlItemByKeyRepository<Blob> sut;
        private readonly Guid key = Guid.NewGuid();

        public SqlItemByKeyRepositoryTest()
        {
            sut = new SqlItemByKeyRepository<Blob>(data.NewContext);
        }

        [Fact]
        public async Task RetreiveItem()
        {
            await using (var db1 = data.NewContext())
            {
                AddBlob(db1, key);
                AddBlob(db1, Guid.NewGuid());
                await db1.SaveChangesAsync();
            }

            var ret = await sut.ItemByKey(key);

            Assert.Equal(key, ret.Key);
            
        }

        private void AddBlob(PlannerDataContext db1, Guid guid)
        {
            db1.Blobs.Add(new Blob() {Key = guid});
        }
    }
}