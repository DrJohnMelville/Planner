using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Moq;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Notes
{
    public class SearchResultPageGeneratorTest
    {
        private readonly Mock<ILocalRepository<Note>> repo = new();
        private readonly SearchResultPageGenerator sut;
        private readonly MemoryStream output = new();
        private string OutputAsString =>
            Encoding.UTF8.GetString(output.ToArray());

        public SearchResultPageGeneratorTest()
        {
            sut = new SearchResultPageGenerator(repo.Object);
        }

        [Fact]
        public async Task DisplayItems()
        {
            repo.Setup(i => i.ItemsByKeys(It.IsAny<IEnumerable<Guid>>())).Returns(
                new ItemList<Note>
                {
                    new() {Title = "Title1"},
                    new() {Title = "Title2"}
                });

            await sut.TryRespond(
            "List/D7F0F0CF-A06F-4E0E-8D4C-6BC5B1E7C49A/D7F0F0CF-A06F-4E0E-8D4C-6BC5B1E7C49A",
                output)!;

            Assert.Equal("Display Notes:Title1\r\nTitle2", OutputAsString);
            
        }

    }
}