using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CefSharp;
using Melville.MVVM.WaitingServices;
using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Notes;
using Planner.WpfViewModels.NotesSearchResults;
using Planner.WpfViewModels.PlannerPages;
using Xunit;

namespace Planner.Wpf.Test.NotesSearchResults
{
    public class NotesSearchViewModelTest
    {
        private readonly Mock<INoteUrlGenerator> urlGen = new();
        private readonly Mock<INoteSearcher> source = new();
        private readonly LocalDate date = new(1975, 07, 28);
        private readonly NotesSearchViewModel sut;
        private readonly EventBroadcast<NoteEditRequestEventArgs> broadcast = new();
        private readonly Mock<IPlannerNavigator> nav = new();
        
        public NotesSearchViewModelTest()
        {
            urlGen.Setup(i => i.ArbitraryNoteView(It.IsAny<IEnumerable<Guid>>())).Returns(
                (IEnumerable<Guid> l) => l.Count().ToString());
            sut = new NotesSearchViewModel(urlGen.Object, Mock.Of<IRequestHandler>(),
                 broadcast, nav.Object);
        }

        [Fact]
        public void VerifyDefaults()
        {
            Assert.Equal("", sut.SearchString);
            Assert.Equal(new LocalDate(1900,01,01), sut.BeginDate);
            Assert.Equal(new LocalDate(3000,01,01), sut.EndDate);
            
        }

        [Fact] 
        public async Task DoSearch()
        {
            sut.SearchString = "Foo";
            sut.BeginDate = date;
            sut.EndDate = date.PlusDays(10);

            source.Setup(i => i.SearchFor("Foo", date, date.PlusDays(10))).Returns(
                TwoResults().ToAsyncEnumerable());

            await sut.DoSearch(source.Object, Mock.Of<IWaitingService>());

            Assert.Equal(2, sut.Results.Count);
            Assert.Equal("Title1", sut.Results[0].Title);
            Assert.Equal("Title2", sut.Results[1].Title);
        }

        private NoteTitle[] TwoResults()
        {
            return new NoteTitle[]
            {
                new("Title1", Guid.Empty, date),
                new("Title2", Guid.Empty, date.PlusDays(2)),
            };
        }

        [Fact]
        public void TestSelection()
        {
            Assert.Equal("0", sut.DisplayUrl);
            sut.NewItemsSelected(TwoResults());
            Assert.Equal("2", sut.DisplayUrl);
        }

        [Fact]
        public void NavToDate()
        {
            sut.NavigatedTo();
            broadcast.Fire(this, new NoteEditRequestEventArgs(Array.Empty<Note>(), new Note(){Date = date}));
            nav.Verify(i=>i.ToDate(date));
            nav.VerifyNoOtherCalls();
        }

    }
}