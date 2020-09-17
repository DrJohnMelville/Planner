using Melville.INPC;
using Planner.Models.Tasks;

namespace Planner.WpfViewModels.TaskList
{
    [AutoNotify]
    public partial class PlannerTaskViewModel
    {
        public PlannerTask PlannerTask {get;}

        public PlannerTaskViewModel(PlannerTask plannerTask)
        {
            PlannerTask = plannerTask;
            ((IExternalNotifyPropertyChanged)this)
                .DelegatePropertyChangeFrom(PlannerTask, nameof(PlannerTask.PriorityDisplay),
                    nameof(ShowPriorityButton), nameof(ShowBlankButton));
        }
        
        public bool ShowPriorityButton => PlannerTask.Priority == ' ';
        public bool ShowBlankButton => !ShowPriorityButton;
    }
}