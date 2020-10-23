using System;

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
}