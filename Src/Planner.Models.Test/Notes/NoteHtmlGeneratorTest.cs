using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
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
        private readonly MemoryStream output = new MemoryStream();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public NoteHtmlGeneratorTest()
        {
            repo.Setup(i => i.CompletedItemsForDate(date)).ReturnsAsync(notes);
            sut = new NoteHtmlGenerator(repo.Object, 
                i=> new JournalItemRenderer(i, new MarkdownTranslator()), new StaticFiles());
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
            Assert.Equal("<html><head><link rel=\"stylesheet\" href=\"journal.css\"></head><body></body></html>", OutputAsString);
            
        }

        private string OutputAsString =>
            Encoding.UTF8.GetString(output.ToArray());
        [Fact]
        public async Task SingleTest()
        {
            notes.Add(new Note{Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("Title", OutputAsString);
            Assert.Contains("Text", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task TitleIsAnchor()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("1.</a>", OutputAsString);
            Assert.Contains("Text", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MarkdownInText()
        {
            notes.Add(new Note(){Title = "Title", Text="**Text**"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("Title", OutputAsString);
            Assert.Contains("<strong>Text</strong>", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MarkdownInTitle()
        {
            notes.Add(new Note(){Title = "**Title**", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("<strong>Title</strong>", OutputAsString);
            Assert.Contains("Text<", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MultTest()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28", output);
            Assert.Contains("<hr/>", OutputAsString);
            Assert.Contains("1.</a>", OutputAsString);
            Assert.Contains("2.</a>", OutputAsString);
        }

        [Fact]
        public async Task SendNoteEditRequest()
        {
            var guid = Guid.NewGuid();
            var note = new Note() {Key = guid};
            notes.Add(note);
            
            var fired = 0;
            sut.NoteEditRequested += (s, e) =>
            {
                fired++;
                Assert.Equal(guid, e.NoteKey);
                Assert.Equal(date, e.Date);
            };

            await sut.GenerateResponse($"{guid}/{date:yyyy-M-d}", output);
            Assert.Equal(1, fired);
            
        }
    }
}