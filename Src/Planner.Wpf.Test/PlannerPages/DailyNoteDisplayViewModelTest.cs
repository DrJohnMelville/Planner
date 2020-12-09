using System;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.PlannerPages;
using Xunit;

namespace Planner.Wpf.Test.PlannerPages
{
    public class DailyNoteDisplayViewModelTest
    {
        private readonly Mock<IClock> clock = new();
        private readonly Mock<ILocalRepository<Note>> noteRepo= new();
        private readonly Mock<INoteUrlGenerator> urlGen = new();
        private readonly DailyNoteDisplayViewModel sut;
        private readonly LocalDate date = new(1975, 07, 28);

        public DailyNoteDisplayViewModelTest()
        {
            sut = new DailyNoteDisplayViewModel(urlGen.Object, date,
                new NoteCreator(noteRepo.Object, clock.Object), Mock.Of<ILinkRedirect>());
        }
        
        [Fact]
        public void NotesUrl()
        {
            urlGen.Setup(i => i.DailyUrl(new LocalDate(1975, 07, 28))).Returns("Url1");
            urlGen.Setup(i => i.DailyUrl(new LocalDate(1975, 07, 29))).Returns("Url2");
            Assert.Equal("Url1", sut.NotesUrl);
        }
        
        [Fact]
        public void CreateNewNote()
        {
            clock.Setup(i => i.GetCurrentInstant()).Returns(Instant.MaxValue);
            var note = new Note();
            noteRepo.Setup(i => i.CreateItem(new LocalDate(1975,07,28),
                It.IsAny<Action<Note>>())).Returns(
                (LocalDate date, Action<Note> action) =>
                {
                    action(note);
                    return note;
                });
            
            sut.NoteCreator.Title = "Title";
            sut.NoteCreator.Text = "Text";

            sut.CreateNoteOnDay();

            Assert.Equal("Title", note.Title);
            Assert.Equal("Text", note.Text);
            Assert.Equal(Instant.MaxValue, note.TimeCreated);

            Assert.Equal("", sut.NoteCreator.Title);
            Assert.Equal("", sut.NoteCreator.Text);
            
        }
        
        [Theory]
        [InlineData("","")]
        [InlineData("  ","   ")]
        [InlineData("","  ")]
        [InlineData("  ","")]
        [InlineData("","klhfs")]
        [InlineData("dwf","")]
        public void NoTextMeansNoNoteCreated(string title, string text)
        {
            sut.NoteCreator.Title = title;
            sut.NoteCreator.Text = text;
            sut.CreateNoteOnDay();
            noteRepo.VerifyNoOtherCalls();
        }
    }
}