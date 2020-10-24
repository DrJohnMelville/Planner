using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Planner.Models.Blobs;
using Planner.Models.Repositories;

namespace Planner.Web.Controllers
{
    [Route("BlobContent")]
    public class BlobContentController : Controller
    {
        private readonly ItemByKeyRepository<Blob> infoSource;
        private readonly IBlobContentStore contentStore;

        public BlobContentController(ItemByKeyRepository<Blob> infoSource, IBlobContentStore contentStore)
        {
            this.infoSource = infoSource;
            this.contentStore = contentStore;
        }

        [HttpGet]
        [Route("{key}")]
        public async Task<IActionResult> Get(Guid key)
        {
            var blob = await infoSource.ItemByKey(key);
            return new FileStreamResult(await contentStore.Read(blob), blob.MimeType);
        }

        [HttpPut]
        [Route("{key}")]
        public async Task<IActionResult> Put(Guid key)
        {
            await contentStore.Write(key, Request.Body);
            return Ok();
        }
    }
}