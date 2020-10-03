using System.Collections.Generic;
using System.Threading.Tasks;
using Melville.MVVM.AdvancedLists;

namespace Planner.Models.Repositories
{
    public interface IListPendingCompletion<T> : IList<T>
    {
        void SetCompletionTask(Task task);
        Task<IList<T>> CompleteList();
    }

    public class ItemList<T> : ThreadSafeBindableCollection<T>, IListPendingCompletion<T>
    {
        private Task completion = Task.CompletedTask;
        public void SetCompletionTask(Task task) => completion = task;

        public async Task<IList<T>> CompleteList()
        {
            await completion;
            return this;
        }
    }
}