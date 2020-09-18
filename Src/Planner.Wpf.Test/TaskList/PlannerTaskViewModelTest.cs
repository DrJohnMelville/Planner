using Microsoft.VisualStudio.TestPlatform.ObjectModel.Utilities;
using Planner.Models.Tasks;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.TaskList
{
    public class PlannerTaskViewModelTest
    {
        private readonly PlannerTaskViewModel sut = new PlannerTaskViewModel(new PlannerTask());

        [Fact]
        public void MarkIncomplete()
        {
            sut.MarkIncomplete();
            Assert.Equal(PlannerTaskStatus.Incomplete, sut.PlannerTask.Status);
            
        }
        [Fact]
        public void MarkDone()
        {
            sut.MarkDone();
            Assert.Equal(PlannerTaskStatus.Done, sut.PlannerTask.Status);
            
        }
        [Fact]
        public void MarkCancelled()
        {
            sut.MarkCanceled();
            Assert.Equal(PlannerTaskStatus.Canceled, sut.PlannerTask.Status);
            
        }
        [Fact]
        public void MarkPending()
        {
            sut.MarkPending();
            Assert.Equal(PlannerTaskStatus.Pending, sut.PlannerTask.Status);
            
        }

        [Fact]
        public void MarkDelegated()
        {
            sut.MarkDelegated();
            Assert.True(sut.PopupOpen);
            Assert.True(sut.PopUpContent is DelegatedContext);
        }
    }
}