using System;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Xunit;

namespace Planner.Models.Test.Notes
{
    public class NoteUrlGeneratorTest
    {
        private readonly Mock<INotesServer> server = new Mock<INotesServer>();
        private readonly INoteUrlGenerator sut;
        private readonly LocalDate date = new LocalDate(1975, 07, 28);
        private readonly Guid guid = new Guid("5c844A5a-2474-4d62-be12-6ac700934bea");
        
        public NoteUrlGeneratorTest()
        {
            server.SetupGet(i => i.BaseUrl).Returns("BaseUrl/");
            sut = new NoteUrlGenerator(server.Object);
        }

        [Fact]
        public void DailyUrl()
        {
            Assert.Equal("BaseUrl/0/1975-7-28/", sut.DailyUrl(date));
            Assert.Equal("BaseUrl/1/1975-7-28/", sut.DailyUrl(date));
            Assert.Equal("BaseUrl/2/1975-7-28/", sut.DailyUrl(date));
        }

        [Fact]
        public void DisplayNoteUrl()
        {
            var note = new Note {Date = date, Key = guid};
            Assert.Equal("BaseUrl/0/1975-7-28/show/5c844a5a-2474-4d62-be12-6ac700934bea", sut.ShowNoteUrl(note));
            Assert.Equal("BaseUrl/1/1975-7-28/show/5c844a5a-2474-4d62-be12-6ac700934bea", sut.ShowNoteUrl(note));
        }
        [Fact]
        public void EditNoteUrl()
        {
            var note = new Note {Date = date, Key = guid};
            Assert.Equal("BaseUrl/0/1975-7-28/5c844a5a-2474-4d62-be12-6ac700934bea", sut.EditNoteUrl(note));
            Assert.Equal("BaseUrl/1/1975-7-28/5c844a5a-2474-4d62-be12-6ac700934bea", sut.EditNoteUrl(note));
        }

        [Fact]
        public void ArbitraryList()
        {
            Assert.Equal("BaseUrl/0/List/5c844a5a-2474-4d62-be12-6ac700934bea/5c844a5a-2474-4d62-be12-6ac700934bea",
                sut.ArbitraryNoteView(new []{guid, guid}));
            Assert.Equal("BaseUrl/1/List/5c844a5a-2474-4d62-be12-6ac700934bea/5c844a5a-2474-4d62-be12-6ac700934bea",
                sut.ArbitraryNoteView(new []{guid, guid}));
        }
    }
}