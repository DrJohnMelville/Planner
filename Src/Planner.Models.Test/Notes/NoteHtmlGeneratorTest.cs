using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Markdown;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Xunit;

namespace Planner.Models.Test.Notes
{
    public class NoteHtmlGeneratorTest
    {
        private readonly Mock<ILocalRepository<Note>> repo = new Mock<ILocalRepository<Note>>();
        private readonly List<Note> notes = new List<Note>();
        private readonly NoteHtmlGenerator sut;
        private readonly StringWriter output = new StringWriter();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public NoteHtmlGeneratorTest()
        {
            repo.Setup(i => i.CompletedItemsForDate(date)).ReturnsAsync(notes);
            sut = new NoteHtmlGenerator(repo.Object, 
                i=> new JournalItemRenderer(i, new MarkdownTranslator()), Mock.Of<IStaticFiles>());
        }

        [Fact]
        public async Task CssTest()
        {
            await sut.GenerateResponse("Journal.css", output);
        }

        [Fact]
        public async Task EmptyTest()
        {
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Equal("<html><head><link rel=\"stylesheet\" href=\"journal.css\"></head><body></body></html>", output.ToString());
            
        }
        [Fact]
        public async Task SingleTest()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("1. Title", output.ToString());
            Assert.Contains("Text", output.ToString());
            Assert.DoesNotContain("<hr/>", output.ToString());
        }
        [Fact]
        public async Task MarkdownInText()
        {
            notes.Add(new Note(){Title = "Title", Text="**Text**"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("1. Title", output.ToString());
            Assert.Contains("<strong>Text</strong>", output.ToString());
            Assert.DoesNotContain("<hr/>", output.ToString());
        }
        [Fact]
        public async Task MarkdownInTitle()
        {
            notes.Add(new Note(){Title = "**Title**", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("1. <strong>Title</strong>", output.ToString());
            Assert.Contains("Text<", output.ToString());
            Assert.DoesNotContain("<hr/>", output.ToString());
        }
        [Fact]
        public async Task MultTest()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("<hr/>", output.ToString());
            Assert.Contains("1. Title", output.ToString());
            Assert.Contains("2. Title", output.ToString());
        }
    }
}