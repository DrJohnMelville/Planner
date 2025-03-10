﻿using Microsoft.EntityFrameworkCore;
using NodaTime;
using Planner.Models.Notes;
using Planner.Models.Tasks;

namespace TUnit.Repository.SqLite;

public class PlannerDataContextTest
{
        
    private readonly TestDatabase data = new TestDatabase();
    [Test]
    public async Task RoundTripPlannerTask()
    {
        await using (var ctx = data.NewContext())
        {
            var pt = new PlannerTask(Guid.Empty)
            {
                Name = "Foo",
                Priority = 'C',
                Order = 3,
                Date = new LocalDate(1975, 07,28)
            };
            ctx.PlannerTasks.Add(pt);
            await ctx.SaveChangesAsync();
        }

        await using var ctx2 = data.NewContext();
        var newPt = await EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(ctx2.PlannerTasks);
        Assert.Equal("Foo", newPt.Name);
        Assert.Equal('C', newPt.Priority);
        Assert.Equal(3, newPt.Order);
        Assert.Equal(new LocalDate(1975,07,28), newPt.Date);
    }

    [Test]
    public async Task RoundTripNote()
    {
        var note = new Note {Key = Guid.NewGuid(), Title = "Name", Text = "Text"};
        using (var ctx = data.NewContext())
        {
            ctx.Notes.Add(note);
            await ctx.SaveChangesAsync();
        }

        using var ctx2 = data.NewContext();
        var note2 = ctx2.Notes.First();
        Assert.Equal(note.Title, note2.Title);
        Assert.Equal(note.Text, note2.Text);
    }
}