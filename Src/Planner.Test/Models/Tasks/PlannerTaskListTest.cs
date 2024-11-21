using Melville.Lists;
using Planner.Models.Tasks;

namespace Planner.Test.Models.Tasks;

public class PlannerTaskListTest
{
    private readonly PlannerTask taskA = new PlannerTask() {Name = "A"};
    private readonly PlannerTask taskB = new PlannerTask() {Name = "B"};
    private readonly PlannerTask taskC = new PlannerTask() {Name = "C"};
    private readonly PlannerTask taskD = new PlannerTask() {Name = "D"};
    private readonly ThreadSafeBindableCollection<PlannerTask> sut = new ThreadSafeBindableCollection<PlannerTask>();

    public PlannerTaskListTest()
    {
        sut.Add(taskA);
        sut.Add(taskB);
        sut.Add(taskC);
        sut.Add(taskD);
    }

    [Test]
    public void AssignPriority()
    {
        Assert.Equal(' ', taskA.Priority);
        sut.PickPriority(taskA, 'A');
        Assert.Equal('A', taskA.Priority);
    }
    [Test]
    public void CyclePriorities()
    {
        Assert.Equal(" ", taskA.PriorityDisplay);
        sut.PickPriority(taskA, 'A');
        Assert.Equal("A", taskA.PriorityDisplay);
        sut.PickPriority(taskA, 'A');
        Assert.Equal("A1", taskA.PriorityDisplay);
        sut.PickPriority(taskA, 'A');
        Assert.Equal(" ", taskA.PriorityDisplay);
    }

    [Test]
    public void AssignOrder()
    {
        sut.PickPriority(taskA, 'A');
        sut.PickPriority(taskB, 'A');
        sut.PickPriority(taskC, 'A');
        Assert.Equal("A", taskA.PriorityDisplay);
        Assert.Equal("A", taskB.PriorityDisplay);
        Assert.Equal("A", taskC.PriorityDisplay);
            
            
        sut.PickPriority(taskA, 'A');
        sut.PickPriority(taskB, 'A');
        sut.PickPriority(taskC, 'A');
        Assert.Equal("A1", taskA.PriorityDisplay);
        Assert.Equal("A2", taskB.PriorityDisplay);
        Assert.Equal("A3", taskC.PriorityDisplay);
    }
    [Test]
    public void AssignOrderWithDifferentPriorities()
    {
        sut.PickPriority(taskA, 'A');
        sut.PickPriority(taskC, 'A');
        sut.PickPriority(taskB, 'B');
        Assert.Equal("A", taskA.PriorityDisplay);
        Assert.Equal("B", taskB.PriorityDisplay);
        Assert.Equal("A", taskC.PriorityDisplay);
            
            
        sut.PickPriority(taskA, 'A');
        sut.PickPriority(taskB, 'A');
        sut.PickPriority(taskC, 'A');
        Assert.Equal("A1", taskA.PriorityDisplay);
        Assert.Equal("B1", taskB.PriorityDisplay);
        Assert.Equal("A2", taskC.PriorityDisplay);
    }
}