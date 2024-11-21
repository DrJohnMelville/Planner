using Melville.TestHelpers.InpcTesting;
using Planner.Models.Tasks;

namespace Planner.Test.Models.Tasks;

public class PlannerTaskTest
{
    private readonly PlannerTask sut = new PlannerTask();
    [Test]
    public void TaskHasName()
    {
        Assert.Equal("", sut.Name);
        sut.Name = "Foo";
        Assert.Equal("Foo", sut.Name);
    }

    [Test]
    public void TaskHasPriority()
    {
        Assert.Equal(' ', sut.Priority);
        sut.Priority = 'A';
        Assert.Equal('A', sut.Priority);
    }

    [Test]
    public void TaskHasOrder()
    {
        Assert.Equal(0, sut.Order);
        sut.Order = 12;
        Assert.Equal(12, sut.Order);
            
    }

    [Test]
    public void TaskHasStatus()
    {
        Assert.Equal(PlannerTaskStatus.Incomplete, sut.Status);
        sut.Status = PlannerTaskStatus.Canceled;
        Assert.Equal(PlannerTaskStatus.Canceled, sut.Status);
               
    }

    [Test]
    public void TaskHasStatusDetail()
    {
        Assert.Equal("", sut.StatusDetail);
        sut.StatusDetail = "7/28/1975";
        Assert.Equal("7/28/1975", sut.StatusDetail);
            
    }

    [Test]
    [Arguments(' ', 0, " ")]
    [Arguments('A', 7, "A7")]
    [Arguments('A', 10, "A10")]
    [Arguments('B', 7, "B7")]
    [Arguments('B', 0, "B")]
    public void PriorityOrder(char priority, int order, string display)
    {
        sut.Priority = priority;
        using var _ = INPCCounter.VerifyInpcFired(sut, 
            i => i.Order, i => i.PriorityDisplay, i=>i.Prioritized);
        sut.Order = order;
        Assert.Equal(display, sut.PriorityDisplay);
    }

    [Test]
    [Arguments('A', 1, "Foo", 'A', 1, "Foo", 0)]
    [Arguments('A', 1, "Foo", ' ', 1, "Foo", 33)]
    [Arguments('C', 1, "Foo", 'A', 1, "Foo", 2)]
    [Arguments('A', 1, "Foo", 'C', 1, "Foo", -2)]
    [Arguments('A', 2, "Foo", 'A', 1, "Foo", 1)]
    [Arguments('A', 1, "Foo", 'A', 2, "Foo", -1)]
    [Arguments('A', 1, "Foo", 'A', 1, "Aoo", 1)]
    [Arguments('A', 1, "Foo", 'A', 1, "Zoo", -1)]
    [Arguments('A', 1, "Foo", 'A', 2, "Bar", -1)]
    public void SortLevel(char p1, int o1, string name1, char p2, int o2, string name2, int comp)
    {
        sut.Priority = p1;
        sut.Order = o1;
        sut.Name = name1;
        var other = new PlannerTask();
        other.Priority = p2;
        other.Order = o2;
        other.Name = name2;

        Assert.Equal(comp, sut.CompareTo(other));
    }

    [Test]
    [Arguments(PlannerTaskStatus.Incomplete, PlannerTaskStatus.Done)]
    [Arguments(PlannerTaskStatus.Done, PlannerTaskStatus.Pending)]
    [Arguments(PlannerTaskStatus.Pending, PlannerTaskStatus.Canceled)]
    [Arguments(PlannerTaskStatus.Canceled, PlannerTaskStatus.Incomplete)]
    [Arguments(PlannerTaskStatus.Delegated, PlannerTaskStatus.Done)]
    [Arguments(PlannerTaskStatus.Deferred, PlannerTaskStatus.Deferred)]
    public void ToggleStatus(PlannerTaskStatus prior, PlannerTaskStatus next)
    {
        sut.Status = prior;
        sut.ToggleStatus();
        Assert.Equal(next, sut.Status);
            
    }

}