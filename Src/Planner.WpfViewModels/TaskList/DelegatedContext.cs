namespace Planner.WpfViewModels.TaskList
{
    public class DelegatedContext : ITaskPopUpContent
    {
        public string Prompt { get; }
        private PlannerTaskViewModel model;

        public DelegatedContext(PlannerTaskViewModel model, string prompt)
        {
            Prompt = prompt;
            this.model = model;
        }

        public string EditText
        {
            get => model.PlannerTask.StatusDetail;
            set => model.PlannerTask.StatusDetail = value;
        }
    }
}