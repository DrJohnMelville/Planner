﻿using System;
using System.Collections.Generic;
using Moq;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Repositories;
using Planner.Models.Tasks;
using Planner.WpfViewModels.Notes;
using Planner.WpfViewModels.PlannerPages;
using Planner.WpfViewModels.TaskList;
using Xunit;

namespace Planner.Wpf.Test.PlannerPages
{
    public class DailyPlannerPageViewModelTest
    {
        private readonly Mock<IClock> clock = new Mock<IClock>();
        private readonly Mock<ILocalRepository<Note>> noteRepo = new Mock<ILocalRepository<Note>>();
        private readonly Mock<ILocalRepository<PlannerTask>> repo = new Mock<ILocalRepository<PlannerTask>>();
        private readonly DailyPlannerPageViewModel sut;

        public DailyPlannerPageViewModelTest()
        {
            repo.Setup(i => i.TasksForDate(It.IsAny<LocalDate>())).Returns(
                (LocalDate d) => new List<PlannerTask>());
            sut = new DailyPlannerPageViewModel(clock.Object, 
                d => new DailyTaskListViewModel(repo.Object, 
                    i=> new PlannerTaskViewModel(i), d),
                d=>new DailyNotesViewModel(noteRepo.Object,d));
        }

        [Fact]
        public void InitialDate()
        {
            repo.Verify(i=>i.TasksForDate(new LocalDate(1969,12,31)), Times.Once);
            repo.VerifyNoOtherCalls();
        }

        [Fact]
        public void ForwardOneDay()
        {
            sut.ForwardOneDay();
            repo.Verify(i=>i.TasksForDate(new LocalDate(1969,12,31)), Times.Once);
            repo.Verify(i=>i.TasksForDate(new LocalDate(1970,1,1)), Times.Once);
            repo.VerifyNoOtherCalls();
            
        }
        [Fact]
        public void BackOneDay()
        {
            sut.BackOneDay();
            repo.Verify(i=>i.TasksForDate(new LocalDate(1969,12,31)), Times.Once);
            repo.Verify(i=>i.TasksForDate(new LocalDate(1969,12,30)), Times.Once);
            repo.VerifyNoOtherCalls();
            
        }
        [Fact]
        public void ArbitraryDate()
        {
            sut.CurrentDate = new LocalDate(1975,07,28);
            repo.Verify(i=>i.TasksForDate(new LocalDate(1969,12,31)), Times.Once);
            repo.Verify(i=>i.TasksForDate(new LocalDate(1975, 07, 28)), Times.Once);
            repo.VerifyNoOtherCalls();
            
        }
    }
}