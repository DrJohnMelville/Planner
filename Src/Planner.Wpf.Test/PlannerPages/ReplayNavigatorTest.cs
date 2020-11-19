using Moq;
using NodaTime;
using Planner.Models.HtmlGeneration;
using Planner.Models.Repositories;
using Planner.WpfViewModels.PlannerPages;
using Xunit;

namespace Planner.Wpf.Test.PlannerPages
{
    public class ReplayNavigatorTest
    {
        private readonly EventBroadcast<ClearCachesEventArgs> signal = new();
        private readonly Mock<IPlannerNavigator> target = new();
        private readonly ReloadingNavigator sut;
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public ReplayNavigatorTest()
        {
            sut = new ReloadingNavigator(target.Object, signal);
        }

        [Fact]
        public void ForwardToDate()
        {
            sut.ToDate(date);
            target.Verify(i=>i.ToDate(date), Times.Once);
            target.VerifyNoOtherCalls();
        }
        [Fact]
        public void ReloadToDate()
        {
            sut.ToDate(date);
            signal.Fire(this, new ClearCachesEventArgs());
            target.Verify(i=>i.ToDate(date), Times.Exactly(2));
            target.VerifyNoOtherCalls();
        }
        [Fact]
        public void ForwardToEditNote()
        {
            NoteEditRequestEventArgs req = new(null!, null!);
            sut.ToEditNote(req);
            target.Verify(i=>i.ToEditNote(req), Times.Once);
            target.VerifyNoOtherCalls();
        }
        [Fact]
        public void ReloadEditNote()
        {
            NoteEditRequestEventArgs req = new(null!, null!);
            sut.ToEditNote(req);
            signal.Fire(this, new ClearCachesEventArgs());
            target.Verify(i=>i.ToEditNote(req), Times.Exactly(2));
            target.VerifyNoOtherCalls();
        }

    }
}