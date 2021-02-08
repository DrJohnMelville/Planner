using System;
using System.Collections.Generic;
using Melville.MVVM.Wpf.KeyboardFacade;
using Microsoft.EntityFrameworkCore.Metadata;
using Moq;
using NodaTime;
using Planner.Models.Appointments;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.Models.Time;
using Planner.Wpf.Appointments;
using Planner.Wpf.PlannerPages;
using Planner.Wpf.PlannerPages;
using Planner.Wpf.TaskList;
using Xunit;
using DailyNoteDisplayViewModel = Planner.Wpf.Notes.DailyNoteDisplayViewModel;
using DailyPlannerPageViewModel = Planner.Wpf.PlannerPages.DailyPlannerPageViewModel;
using DailyTaskListViewModel = Planner.Wpf.TaskList.DailyTaskListViewModel;
using PlannerTaskViewModel = Planner.Wpf.TaskList.PlannerTaskViewModel;

namespace Planner.Wpf.Test.PlannerPages
{
    public class DailyPlannerPageViewModelTest
    {
        private readonly Mock<IPlannerNavigator> navigation = new();
        private readonly Mock<ILocalRepository<PlannerTask>> repo = new();
        private readonly EventBroadcast<NoteEditRequestEventArgs> requestEdit = new();
        private readonly DailyPlannerPageViewModel sut;
        

        public DailyPlannerPageViewModelTest()
        {
            repo.Setup(i => i.ItemsForDate(It.IsAny<LocalDate>())).Returns(
                (LocalDate d) => new ItemList<PlannerTask>());
            Func<LocalDate, DailyTaskListViewModel> taskListFactory = 
                d => new DailyTaskListViewModel(repo.Object, i=> new PlannerTaskViewModel(i),
                    Mock.Of<IKeyboardQuery>(), d);
            Func<LocalDate, DailyNoteDisplayViewModel> notesCreator = d => 
                new DailyNoteDisplayViewModel(null, d, null, null);
            
            sut = new DailyPlannerPageViewModel(new LocalDate(1975,07,28), 
                taskListFactory, notesCreator,
                d=>new DailyAppointmentsViewModel(d, 
                    Mock.Of<ILocalRepository<Appointment>>(), Mock.Of<IPlannerNavigator>()),
                        navigation.Object, requestEdit, Mock.Of<ILinkRedirect>(),
                   i=>new RichTextCommandTarget(null,null,LocalDate.MaxIsoValue));
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
        public void ReloadCachesTest()
        {
            Mock<IEventBroadcast<ClearCachesEventArgs>> signal = new();
            sut.ReloadCaches(signal.Object);
            signal.Verify(i=>i.Fire(sut, It.IsAny<ClearCachesEventArgs>()), Times.Once);
        }

        [Fact]
        public void GoToTodayTest()
        {
            var clock = new Mock<IUsersClock>();
            var dateTimeZone = DateTimeZoneProviders.Tzdb.GetSystemDefault();
            clock.Setup(i => i.CurrentDate()).Returns(new LocalDate(1974,08,18));
            sut.GoToToday(clock.Object);
            navigation.Verify(i=>i.ToDate(new LocalDate(1974,08,18)), Times.Once);
        }

        [Fact]
        public void SearchJournalCommand()
        {
            sut.SearchJournal();
            navigation.Verify(i=>i.ToNoteSearchPage(), Times.Once);
        }
    }
}