using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using Melville.MVVM.AdvancedLists;

namespace Planner.Models.Repositories
{
    public interface IListPendingCompletion<T> : IList<T>
    {
        public bool FinishedLoading { get; }
        void SetCompletionTask(Task task);
        Task<IListPendingCompletion<T>> CompleteList();
    }

    public class ItemList<T> : ThreadSafeBindableCollection<T>, IListPendingCompletion<T>
    {
        public bool FinishedLoading => completion.IsCompleted;
        private Task completion = Task.CompletedTask;
        public void SetCompletionTask(Task task)
        {
            completion = task;
            FinishedLoadingChanged();
            completion.ContinueWith(t => FinishedLoadingChanged(), TaskContinuationOptions.None);
        }

        private void FinishedLoadingChanged() =>
            OnPropertyChanged(new PropertyChangedEventArgs(nameof(FinishedLoading)));

        public async Task<IListPendingCompletion<T>> CompleteList()
        {
            await completion;
            return this;
        }
    }
}