using System;
using Melville.SystemInterface.Time;

namespace Planner.Models.HtmlGeneration
{
    public interface IEventBroadcast<T> where T : EventArgs
    {
        event EventHandler<T>? Fired;
        void Fire(object? sender, T args);
    }
    public class EventBroadcast<T>: IEventBroadcast<T> where T : EventArgs
    {
        public event EventHandler<T>? Fired;
        public void Fire(object? sender, T args) => Fired?.Invoke(sender, args);
    }

    public class DelayedEventBroadcast<TSource> : EventBroadcast<TSource>
      where TSource:EventArgs
    {
        private readonly IWallClock clock;
        public DelayedEventBroadcast(IEventBroadcast<TSource> source, IWallClock clock)
        {
            this.clock = clock;
            source.Fired += WaitAndForward;
        }

        private async void WaitAndForward(object? sender, TSource e)
        {
            await clock.Wait(TimeSpan.FromSeconds(0.5));
            Fire(sender, e);
        }
    }
}