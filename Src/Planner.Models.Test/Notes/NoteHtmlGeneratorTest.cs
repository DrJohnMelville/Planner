﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestPlatform.Utilities;
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
        private readonly ItemList<Note> notes = new ItemList<Note>();
        private readonly NoteHtmlGenerator sut;
        private readonly MemoryStream output = new MemoryStream();
        private readonly Mock<INoteUrlGenerator> urlGen = new Mock<INoteUrlGenerator>();
        private readonly Mock<ILocalRepository<Blob>> blobRepo = new Mock<ILocalRepository<Blob>>();
        private readonly Mock<IBlobContentStore> blobStore = new Mock<IBlobContentStore>();
        private readonly Mock<IEventBroadcast<NoteEditRequestEventArgs>> broadcast =
            new Mock<IEventBroadcast<NoteEditRequestEventArgs>>();
        

        private readonly LocalDate date = new LocalDate(1975,07,28);

        public NoteHtmlGeneratorTest()
        {
            repo.Setup(i => i.ItemsForDate(date)).Returns(notes);
            sut = new NoteHtmlGenerator(new List<ITryNoteHtmlGenerator>()
                {
                    new StaticFileGenerator(),
                    new BlobGenerator(blobRepo.Object, blobStore.Object),
                    new DailyJournalPageGenerator(
                        i=> new JournalItemRenderer(i, d=>new MarkdownTranslator(d), urlGen.Object),
                        repo.Object),
                    new DefaultTextGenerator()
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
        [Theory]
        [InlineData("(1.2.3)", "<a href='/navToPage/1975-1-2'>(1.2.3)</a>")]
        [InlineData("(1.2.1975.3)", "<a href='/navToPage/1975-1-2'>(1.2.1975.3)</a>")]
        [InlineData("(1.2.75.3)", "<a href='/navToPage/1975-1-2'>(1.2.75.3)</a>")]
        public async Task DateLinkInText(string link, string expected)
        {
            notes.Add(new Note(){Title = "Title", Text="See "+link, Date = date});
            await sut.GenerateResponse("1975-7-28/", output);
            Assert.Contains(expected, OutputAsString);
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
            
            await sut.GenerateResponse($"{date:yyyy-M-d}/{guid}", output);
            
            broadcast.Verify(i=>i.Fire(It.IsAny<object>(), It.Is<NoteEditRequestEventArgs>(i=>
                i.Note == note && i.DailyList == notes)), Times.Once());
        }

        [Theory]
        [InlineData("1975-7-28/3.2_1")]
        [InlineData("1975-7-28/show/3.2_1")]
        public async Task RetrieveImage(string url)
        {
            var foundBlob = new Blob();
            blobRepo.Setup(i => i.ItemsForDate(new LocalDate(1975, 3, 2))).Returns(
                new ItemList<Blob>() {foundBlob});
            blobStore.Setup(i=>i.Read(foundBlob))
                .ReturnsAsync(new MemoryStream(Encoding.UTF8.GetBytes("From Blob")));
            await sut.GenerateResponse(url, output);
            Assert.Equal("From Blob", OutputAsString);
        }

    }
}