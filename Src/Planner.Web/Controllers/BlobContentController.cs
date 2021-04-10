using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using NodaTime;
using Planner.Models.Blobs;

namespace Planner.Web.Controllers
{
    [Route("BlobContent")]
    public class BlobContentController : Controller
    {
        [HttpGet]
        [Route("{key}")]
        public async Task<IActionResult> Get(Guid key, [FromServices]BlobStreamExtractor extractor)
        {
            var pair = await extractor.FromGuid(key);
            return FormatOutput(pair);
        }

        private static FileStreamResult FormatOutput((Stream Stream, string MimeType) pair)
        {
            return new FileStreamResult(pair.Stream, pair.MimeType);
        }

        private static readonly Regex contentParser = new Regex(@"^(\d+)\.(\d+)(?:\.(\d+))?_(\d+)$");
        [HttpGet]
        [Route("{date}/{contentString}")]
        public async Task<IActionResult> Get(LocalDate date, string contentString, 
            [FromServices]BlobStreamExtractor extractor)
        {
            var match = contentParser.Match(contentString);
            return match.Success
                ? FormatOutput(await extractor.FromComponents(date,
                    match.Groups[3].Value, match.Groups[1].Value, match.Groups[2].Value, match.Groups[4].Value))
                : (IActionResult) NotFound();
        }

        [HttpPut]
        [Route("{key}")]
        public async Task<IActionResult> Put(Guid key, [FromServices]IBlobContentStore contentStore)
        {
            await contentStore.Write(key, Request.Body);
            return Ok();
        }
    }
}