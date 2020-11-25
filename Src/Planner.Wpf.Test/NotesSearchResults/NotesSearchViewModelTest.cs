using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query.Internal;
using Moq;
using NodaTime;
using Planner.Models.Notes;
using Planner.WpfViewModels.NotesSearchResults;
using Xunit;

namespace Planner.Wpf.Test.NotesSearchResults
{
    public class NotesSearchViewModelTest
    {
        private readonly Mock<INoteSearcher> source = new Mock<INoteSearcher>();
        private readonly LocalDate date = new LocalDate(1975, 07, 28);
        private readonly NotesSearchViewModel sut;

        public NotesSearchViewModelTest()
        {
            sut = new NotesSearchViewModel();
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

            source.Setup(i => i.SearchFor("Foo", date, date.PlusDays(10))).Returns(new NoteTitle[]
            {
              new("Title1", Guid.Empty, date),
              new("Title2", Guid.Empty, date.PlusDays(2)),
            }.ToAsyncEnumerable());

            await sut.DoSearch(source.Object);

            Assert.Equal(2, sut.Results.Count);
            Assert.Equal("Title1", sut.Results[0].Title);
            Assert.Equal("Title2", sut.Results[1].Title);
            
            
        }

    }
}