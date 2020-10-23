﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Moq;
using NodaTime;
using Planner.Models.Blobs;
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
        private readonly Mock<INoteUrlGenerator> urlGen = new Mock<INoteUrlGenerator>();
        private readonly Mock<ILocalRepository<Blob>> blobRepo = new Mock<ILocalRepository<Blob>>();
        private readonly Mock<IBlobContentStore> blobStore = new Mock<IBlobContentStore>();

        private readonly LocalDate date = new LocalDate(1975,07,28);

        public NoteHtmlGeneratorTest()
        {
            repo.Setup(i => i.CompletedItemsForDate(date)).ReturnsAsync(notes);
            sut = new NoteHtmlGenerator(repo.Object, 
                new List<IHtmlContentOption>()
                {
                    new StaticFiles(),
                    new BlobContentOption(blobRepo.Object, blobStore.Object),
                    new DailyJournalPageContent(
                        i=> new JournalItemRenderer(i, new MarkdownTranslator(), urlGen.Object),
                        repo.Object),
                    new DefaultText()
                });
        }

        [Fact]
        public async Task CssTest()
        {
            await sut.GenerateResponse("Journal.css", output);
        }

        [Fact]
        public async Task EmptyTest()
        {
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Equal("<html><head><link rel=\"stylesheet\" href=\"/0/journal.css\"></head><body>", OutputAsString);
            
        }

        private string OutputAsString =>
            Encoding.UTF8.GetString(output.ToArray());
        [Fact]
        public async Task SingleTest()
        {
            notes.Add(new Note{Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("Title", OutputAsString);
            Assert.Contains("Text", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
            Assert.DoesNotContain("<script", OutputAsString);
        }
        [Fact]
        public async Task MermaidTest()
        {
            notes.Add(new Note{Title = "Title", Text=@"````mermaid
````"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("div class=\"mermaid\">", OutputAsString);
            Assert.Contains("<script src=\"https://cdn.jsdelivr.net/npm/mermaid/dist/mermaid.min.js\"></script><script>mermaid.initialize({startOnLoad:true});</script>", OutputAsString);
        }
        [Fact]
        public async Task TitleIsAnchor()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("1.</a>", OutputAsString);
            Assert.Contains("Text", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MarkdownInText()
        {
            notes.Add(new Note(){Title = "Title", Text="**Text**"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("Title", OutputAsString);
            Assert.Contains("<strong>Text</strong>", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MarkdownInTitle()
        {
            notes.Add(new Note(){Title = "**Title**", Text="Text"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("<strong>Title</strong>", OutputAsString);
            Assert.Contains("Text<", OutputAsString);
            Assert.DoesNotContain("<hr/>", OutputAsString);
        }
        [Fact]
        public async Task MultTest()
        {
            notes.Add(new Note(){Title = "Title", Text="Text"});
            notes.Add(new Note(){Title = "Title", Text="Text"});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains("<hr/>", OutputAsString);
            Assert.Contains("1.</a>", OutputAsString);
            Assert.Contains("2.</a>", OutputAsString);
        }
        [Fact]
        public async Task SingleNoteFromList()
        {
            var key = Guid.NewGuid();
            notes.Add(new Note(){Title = "Title", Text="Text"});
            notes.Add(new Note(){Title = "Title", Text="Text", Key = key});
            await sut.GenerateResponse("1975-7-28/show/"+key, output);
            Assert.DoesNotContain("1.</a>", OutputAsString);
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
                Assert.Equal(note, e.Note);
                Assert.Equal(notes, e.DailyList);
            };

            await sut.GenerateResponse($"{date:yyyy-M-d}/{guid}", output);
            Assert.Equal(1, fired);
        }

        [Fact]
        public async Task RetrieveImage()
        {
            var foundBlob = new Blob();
            blobRepo.Setup(i => i.CompletedItemsForDate(new LocalDate(1975, 3, 2))).ReturnsAsync(
                new List<Blob>() {foundBlob});
            blobStore.Setup(i=>i.Read(foundBlob))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("From Blob")));
            await sut.GenerateResponse("1975-7-28/3.2_1", output);
            Assert.Equal("From Blob", OutputAsString);
        }

    }
}