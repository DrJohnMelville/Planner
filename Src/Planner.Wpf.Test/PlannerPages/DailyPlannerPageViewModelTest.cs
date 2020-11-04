using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Melville.MVVM.Wpf.RootWindows;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.Notes.Pasters;
using Planner.WpfViewModels.PlannerPages;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.PlannerPages
{
    public class DailyPlannerPageViewModelTest
    {
        private readonly Mock<IClock> clock = new Mock<IClock>();
        private readonly Mock<IPlannerNavigator> navigation = new Mock<IPlannerNavigator>();
        private readonly Mock<INotesServer> notes = new Mock<INotesServer>();
        private readonly Mock<ILocalRepository<Note>> noteRepo= new Mock<ILocalRepository<Note>>();
        private readonly Mock<ILocalRepository<PlannerTask>> repo = new Mock<ILocalRepository<PlannerTask>>();
        private readonly Mock<INoteUrlGenerator> urlGen = new Mock<INoteUrlGenerator>();
        private readonly EventBroadcast<NoteEditRequestEventArgs> requestEdit = new EventBroadcast<NoteEditRequestEventArgs>();
        private readonly DailyPlannerPageViewModel sut;
        

        public DailyPlannerPageViewModelTest()
        {
            repo.Setup(i => i.ItemsForDate(It.IsAny<LocalDate>())).Returns(
                (LocalDate d) => new List<PlannerTask>());
            Func<LocalDate, DailyTaskListViewModel> taskListFactory = d => new DailyTaskListViewModel(repo.Object, i=> new PlannerTaskViewModel(i), d);
            var noteCreator = new NoteCreator(noteRepo.Object, clock.Object);
            sut = new DailyPlannerPageViewModel(new LocalDate(1975,07,28), 
                taskListFactory,
                     notes.Object, 
                         noteCreator,
                        navigation.Object, 
                urlGen.Object, requestEdit
                );
        }

        [Fact]
        public void InitialDate()
        {
            repo.Verify(i=>i.ItemsForDate(new LocalDate(1975, 07, 28)), Times.Once);
            repo.VerifyNoOtherCalls();
        }

        [Fact]
        public void ForwardOneDay()
        {
            sut.ForwardOneDay();
            navigation.Verify(i=>i.ToDate(new LocalDate(1975,07,29)));

        }
        [Fact]
        public void BackOneDay()
        {
            sut.BackOneDay();
            navigation.Verify(i=>i.ToDate(new LocalDate(1975,07,27)));
        }
        [Fact]
        public void ArbitraryDate()
        {
            var date = new LocalDate(1975,01,28);
            sut.CurrentDate = date;
            navigation.Verify(i=>i.ToDate(date));
        }

        [Fact]
        public void NotesUrl()
        {
            urlGen.Setup(i => i.DailyUrl(new LocalDate(1975, 07, 28))).Returns("Url1");
            urlGen.Setup(i => i.DailyUrl(new LocalDate(1975, 07, 29))).Returns("Url2");
            sut.CurrentDate = new LocalDate(1975,07,28);
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
            sut.CurrentDate = new LocalDate(1975,07,28);
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

        [Theory]
        [InlineData(false, false, 0)]
        [InlineData(true, false, 1)]
        [InlineData(true, true, 0)]
        public void ReceivesNoteEditRequests(bool attached, bool detached, int calls)
        {
            if (attached) sut.NavigatedTo();
            if (detached) sut.NavigatedAwayFrom();
            requestEdit.Fire(this, new NoteEditRequestEventArgs(
                new List<Note>(), new Note()));
            navigation.Verify(i=>i.ToEditNote(It.IsAny<NoteEditRequestEventArgs>()), Times.Exactly(calls));
        }
        
        [Fact]
        public void PlannerPagerLink3()
        {
            sut.CurrentDate = new LocalDate(1975,07,28);
            var match = Regex.Match("(1.2.3)", @"\((\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.3)", "(1.2.3)", TaskTextType.PlannerPage,
                match));
            navigation.Verify(i=>i.ToDate(new LocalDate(1975,1,2)));
            
        }
        [Fact]
        public void PlannerPagerLink4DightYeat()
        {
            sut.CurrentDate = new LocalDate(1975,07,28);
            var match = Regex.Match("(1.2.1980.3)", @"\((\d+)\.(\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.1980.3)", "(1.2.1980.3)",
                TaskTextType.PlannerPage, match));
            navigation.Verify(i=>i.ToDate(new LocalDate(1980,1,2)));
        }
        [Fact]
        public void PlannerPagerLink2DigitYear()
        {
            sut.CurrentDate = new LocalDate(1975,07,28);
            var match = Regex.Match("(1.2.80.3)", @"\((\d+)\.(\d+)\.(\d+)\.(\d+)\)");
            sut.PlannerPageLinkClicked(new Segment<TaskTextType>("(1.2.80.3)", "(1.2.80.3)", TaskTextType.PlannerPage,
                match));
            navigation.Verify(i=>i.ToDate(new LocalDate(1980,1,2)));
        }
    }
}