using System;
using System.Collections.Generic;
using Melville.MVVM.Wpf.RootWindows;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.WpfViewModels.Notes;
using Xunit;

namespace Planner.Wpf.Test.Notes
{
    public class NoteEditorViewModelTest
    {
        private readonly Mock<INotesServer> noteServer = new Mock<INotesServer>();
        private readonly Note note;
        private readonly IList<Note> notes = new List<Note>();
        private readonly Mock<INavigationWindow> navWin = new Mock<INavigationWindow>();
        private readonly NoteEditorViewModel sut;

        private readonly ZonedDateTime creationTime = new LocalDateTime(1975, 07, 28, 0, 0, 0)
            .InZoneLeniently(DateTimeZoneProviders.Tzdb.GetSystemDefault());
                

        public NoteEditorViewModelTest()
        {
            noteServer.SetupGet(i => i.BaseUrl).Returns("url://");
            note = new Note()
            {
                Key = Guid.NewGuid(),
                Title = "Title",
                Text = "**Text**",
                TimeCreated = creationTime.ToInstant()
            };
            notes.Add(note);
            sut = new NoteEditorViewModel(new NoteEditRequestEventArgs(notes, note),
                new NoteUrlGenerator(noteServer.Object), navWin.Object, i=>null);
        }

        [Fact]
        public void CreationDateDisplay()
        {
            Assert.Equal("Note Created: Monday, July 28, 1975 12:00 AM", sut.DisplayCreationDate);
        }

        [Fact]
        public void DisplayUrlTest()
        {
            Assert.Equal($"url://0/{note.Date:yyyy-M-d}/{note.Key}", sut.NoteUrl);
            Assert.Equal($"url://1/{note.Date:yyyy-M-d}/{note.Key}", sut.NoteUrl);
            Assert.Equal($"url://2/{note.Date:yyyy-M-d}/{note.Key}", sut.NoteUrl);
        }

        [Fact]
        public void UpdateUrlOnNoteChange()
        {
            int changes = 0;
            sut.PropertyChanged += (s, e) =>
            {
                Assert.Equal(nameof(NoteEditorViewModel.NoteUrl), e.PropertyName);
                changes++;
            };

            Assert.Equal(0, changes);
            note.Title = "Foo";
            Assert.Equal(1, changes);
            note.Text = "Bar";
            Assert.Equal(2, changes);
        }

        [Fact]
        public void OKTest()
        {
            sut.NavigateToPlannerPage();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
        }

        [Fact]
        public void CancelRevertsTitle()
        {
            note.Title = "Foo";
            sut.CancelEdit();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
            Assert.Equal("Title", note.Title);
            
        }        
        
        [Fact]
        public void CancelRevertsText()
        {
            note.Text = "Foo";
            sut.CancelEdit();
            navWin.Verify(i=>i.NavigateTo(null), Times.Once);
            navWin.VerifyNoOtherCalls();
            Assert.Equal("**Text**", note.Text);
        }
    }
}