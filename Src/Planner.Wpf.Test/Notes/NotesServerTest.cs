using System;
using System.Collections.Generic;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.WpfViewModels.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class NotesServerTest
    {
        private readonly Mock<INoteHtmlGenerator> generator = new Mock<INoteHtmlGenerator>();
        public readonly NotesServer sut;

        public NotesServerTest()
        {
            sut = new NotesServer(generator.Object);
        }

        [Fact]
        public void CorrectUrl()
        {
            Assert.Equal("http://localhost:28775/", sut.BaseUrl);
        }

        [Fact]
        public void NoteEditRequestredForwards()
        {
            var fired = 0;
            RaiseEditNoteRequest();
            Assert.Equal(0, fired);
            void updateNote(object s, NoteEditRequestEventArgs e) => fired++;
            sut.NoteEditRequested += updateNote;
            RaiseEditNoteRequest();
            Assert.Equal(1, fired);
            sut.NoteEditRequested -= updateNote;
            RaiseEditNoteRequest();
            Assert.Equal(1, fired);
        }

        private void RaiseEditNoteRequest() =>
            generator.Raise(i => i.NoteEditRequested -= null,
                new NoteEditRequestEventArgs(new List<Note>(),
                    new Note()));
    }
}