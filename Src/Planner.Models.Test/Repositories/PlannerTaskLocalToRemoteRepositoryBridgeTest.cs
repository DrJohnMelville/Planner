﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Melville.MVVM.Time;
using Melville.MVVM.WaitingServices;
using Melville.TestHelpers.MockConstruction;
using Moq;
using NodaTime;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Planner.Models.Test.Repositories
{
    public class PlannerTaskLocalToRemoteRepositoryBridgeTest
    {
        private readonly Mock<IRemotePlannerTaskRepository> repo = 
            new Mock<IRemotePlannerTaskRepository>();

        private readonly Mock<IWallClock> clock = new Mock<IWallClock>();
        private readonly LocalDate date = new LocalDate(1975,07,28);

        public PlannerTaskLocalToRemoteRepositoryBridge sut;

        public PlannerTaskLocalToRemoteRepositoryBridgeTest()
        {
            sut = new PlannerTaskLocalToRemoteRepositoryBridge(repo.Object, clock.Object);
        }

        [Fact]
        public void CreateTaskUpdatesRemoteRepository()
        {
            var task = sut.CreateTask("Foo", date);
            repo.Verify(i=>i.AddTask((RemotePlannerTask)task), Times.Once);
        }

        [Fact]
        public void ModifyFieldUpdatesTask()
        {
            var task = sut.CreateTask("Foo", date);
            task.Name = "Bar";
            repo.Verify(i=>i.AddTask((RemotePlannerTask)task), Times.Once);
            repo.Verify(i=>i.UpdateTask((RemotePlannerTask)task), Times.Once);
        }

        [Fact]
        public void CollapseMultipleUpdates()
        {
            var tcs = new TaskCompletionSource<int>();
            clock.Setup(I => I.Wait(It.IsAny<TimeSpan>())).Returns(tcs.Task);
            var task = sut.CreateTask("Foo", date);
            task.Name = "Bar";
            task.Priority = 'A';
            task.Order = 1;
            tcs.SetResult(1);
            repo.Verify(i=>i.AddTask((RemotePlannerTask)task), Times.Once);
            repo.Verify(i=>i.UpdateTask((RemotePlannerTask)task), Times.Once);
        }

        private async IAsyncEnumerable<T> AsyncEnum<T>(params T[] items)
        {
            await Task.CompletedTask; // shut the compiler warning up
            foreach (var item in items)
            {
                yield return item;
            }
        }

        [Fact]
        public void ChangingALoadedTaskCausesAnUpdate()
        {
            var task = new RemotePlannerTask(Guid.NewGuid());
            repo.Setup(i => i.TasksForDate(date)).Returns(AsyncEnum(task));
            var list = sut.TasksForDate(date);
            Assert.Single(list);
            list[0].Name = "Bar";
            repo.Verify(i=>i.UpdateTask((RemotePlannerTask)list[0]), Times.Once);
        }
        [Fact]
        public void RemovingTaskDeletesFromDatabase()
        {
            var task = new RemotePlannerTask(Guid.NewGuid());
            repo.Setup(i => i.TasksForDate(date)).Returns(AsyncEnum(task));
            var list = sut.TasksForDate(date);
            Assert.Single(list);
            var item = list[0];
            list.RemoveAt(0);
            repo.Verify(i=>i.DeleteTask((RemotePlannerTask)item), Times.Once);
        }
    }
}